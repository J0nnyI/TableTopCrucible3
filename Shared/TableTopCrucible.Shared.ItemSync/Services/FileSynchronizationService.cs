using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using DynamicData;
using MoreLinq.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.ItemSync.Models;
using TableTopCrucible.Shared.Services;

namespace TableTopCrucible.Shared.ItemSync.Services;

/// <summary>
///     Synchronizes the master file list with the files in the directory
/// </summary>
[Singleton]
public interface IFileSynchronizationService
{
    ICommand StartScanCommand { get; }
    ITrackingViewer StartScan();
}

public class FileSynchronizationService : IFileSynchronizationService
{
    private readonly IDirectorySetupRepository _directorySetupRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IProgressTrackingService _progressTrackingService;
    private readonly IThumbnailGenerationService _thumbnailGenerationService;
    private readonly IImageRepository _imageRepository;

    private readonly object _itemUpdateLocker = new();

    public FileSynchronizationService(
        IDirectorySetupRepository directorySetupRepository,
        IFileRepository fileRepository,
        IItemRepository itemRepository,
        IProgressTrackingService progressTrackingService,
        IThumbnailGenerationService thumbnailGenerationService,
        IImageRepository imageRepository)
    {
        _directorySetupRepository = directorySetupRepository;
        _fileRepository = fileRepository;
        _itemRepository = itemRepository;
        _progressTrackingService = progressTrackingService;
        _thumbnailGenerationService = thumbnailGenerationService;
        _imageRepository = imageRepository;

        StartScanCommand = ReactiveCommand.Create(
            StartScan,
            this.WhenAnyValue(s => s.ScanRunning)
                .Select(x => !x)
                .ObserveOn(RxApp.MainThreadScheduler));
    }

    [Reactive]
    public bool ScanRunning { get; private set; }

    public ICommand StartScanCommand { get; }

    public ITrackingViewer StartScan()
    {
        if (ScanRunning)
            return null;
        ScanRunning = true;

        var totalProgress = _progressTrackingService
            .CreateCompositeTracker(
                (Name)"File Synchronization"
            );
        var stepTracker = totalProgress.AddSingle((Name)"Total", (TargetProgress)1, (JobWeight)1);
        var hashingTracker = totalProgress.AddSingle((Name)"hashing", null, (JobWeight)10);
        var updateTracker = totalProgress.AddSingle((Name)"update", null, (JobWeight)3);
        var thumbnailGenerationTracker =
            SettingsHelper.GenerateThumbnailOnSync
                ? totalProgress.AddSingle((Name)"Thumbnail Generation", null, (JobWeight)30)
                : null;

        var syncDisposables = new CompositeDisposable();
        totalProgress
            .OnDone()
            .Take(1)
            .Subscribe(_ =>
            {
                ScanRunning = false;
                syncDisposables.Dispose();
            });

        Observable.Start(() =>
        {
            try
            {
                var files = getFileGroups(_directorySetupRepository.Data.Items);

                stepTracker.Increment();

                // remove deleted files
                _fileRepository.RemoveRange(files.DeletedFiles.Select(file => file.KnownFile));

                // prepare hash process
                var filesToHash = files.NewFiles
                    .Concat(files.UpdatedFiles)
                    .ToArray();

                if (filesToHash.Length > 0)
                {
                    var totalSize = filesToHash.Sum(file => file.FoundFile.Value.Length);
                    hashingTracker.SetTarget((TargetProgress)totalSize);
                    updateTracker.SetTarget((TargetProgress)filesToHash.Length);
                    thumbnailGenerationTracker?.SetTarget(
                        (TargetProgress)filesToHash
                            .Where(file => file.UpdateSource == FileUpdateSource.New && file.FoundFile.IsModel())
                            .Sum(file => file.FoundFile.GetSize().Value));

                    var fileUpdatePipeline = new Subject<RawSyncFileData>().DisposeWith(syncDisposables);
                    var thumbnailPipeline = new ReplaySubject<FileData>().DisposeWith(syncDisposables);

                    fileUpdatePipeline
                        .ObserveOn(RxApp.TaskpoolScheduler)
                        .Buffer(SettingsHelper.PipelineBufferTime)
                        .Where(buffer => buffer.Any())
                        .TakeUntil(updateTracker.OnDone())
                        .Subscribe(fileData =>
                        {
                            _handleChangedFiles(fileData.ToArray(), thumbnailPipeline);
                            updateTracker.Increment((ProgressIncrement)fileData.Count);
                        });

                    if (SettingsHelper.GenerateThumbnailOnSync)
                        _thumbnailGenerationService.GenerateManyAsync(thumbnailPipeline,
                            thumbnailGenerationTracker);

                    // hash items and do the rest in parallel
                    filesToHash
                        .AsParallel()
                        .WithDegreeOfParallelism(SettingsHelper.FileHashThreadCount)
                        .ForAll(fileData =>
                        {
                            fileData.CreateNewHashKey();
                            hashingTracker.Increment((ProgressIncrement)fileData.FoundFile.Value.Length);
                            lock (fileUpdatePipeline)
                            {
                                fileUpdatePipeline.OnNext(fileData);
                            }
                        });
                }
                else
                {
                    thumbnailGenerationTracker?.Complete();
                    hashingTracker.Complete();
                    updateTracker.Complete();
                    stepTracker.Complete();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debugger.Break();
                stepTracker.Complete();
                hashingTracker.Complete();
                updateTracker.Complete();
                thumbnailGenerationTracker?.Complete();
                throw;
            }
        }, RxApp.TaskpoolScheduler);

        return totalProgress;
    }

    private IEnumerable<RawSyncFileData> startScanForDirectory(DirectorySetup directory)
    {
        var foundFiles = directory.Path.GetFiles(FileType.Image, FileType.Model).ToArray();
        var knownFiles = _fileRepository[directory.Path];
        return foundFiles.FullJoin(knownFiles,
            foundFile => foundFile.Value.ToLower(),
            knownFile => knownFile.Path.Value.ToLower(),
            found => new RawSyncFileData(null, found),
            known => new RawSyncFileData(known, null),
            (found, known) => new RawSyncFileData(known, found));
    }

    private FileSyncListGrouping getFileGroups(IEnumerable<DirectorySetup> directorySetups)
    {
        var dirs = _directorySetupRepository.Data.Items.Select(dir => dir.Path);
        var foundFiles = directorySetups
            .AsEnumerable()
            .SelectMany(startScanForDirectory)
            .ToArray();

        var filesOfUnregisteredDirs = _fileRepository
            .Data
            .Items
            .Where(file => !dirs.Any(dir => file.Path.Value.ToLower().StartsWith(dir.Value.ToLower())))
            .Select(file => new RawSyncFileData(file, null))
            .ToArray();

        var groupings = new FileSyncListGrouping(foundFiles.Concat(filesOfUnregisteredDirs));

        return groupings;
    }

    private void _handleChangedFiles(RawSyncFileData[] files, ISubject<FileData> thumbnailPipeline)
    {
        var filesToAdd = files
            .Where(x => x.UpdateSource == FileUpdateSource.New)
            .Select(file => file.GetNewEntity()).ToArray();
        _fileRepository.AddRange(filesToAdd);
        lock (_itemUpdateLocker)
        {
            var modelFiles = filesToAdd
                .Where(file => file.Path.GetExtension().IsModel())
                .ToArray();
            var itemsToAdd = Enumerable.DistinctBy(modelFiles, x => x.HashKey)
                .Where(file => _itemRepository.ByModelHash(file.HashKey).None())
                .Select(file => new Item(
                        file.Path.GetFilenameWithoutExtension().ToName(),
                        file.HashKey
                    )
                ).ToArray();


            modelFiles.ToList().ForEach(modelFile =>
            {
                var tags = GetTagsByPath(modelFile.Path);
                var items =
                    itemsToAdd.Where(item => item.FileKey3d == modelFile.HashKey).ToList();
                if (!items.Any())
                    items = _itemRepository.ByModelHash(modelFile.HashKey).ToList();
                items.ForEach(item => item.Tags.AddRange(tags.ToArray()));
            });

            _itemRepository.AddRange(itemsToAdd);
            modelFiles.ToList().ForEach(thumbnailPipeline.OnNext);
        }
    }

    private IEnumerable<Tag> GetTagsByPath(FilePath filePath)
    {
        var dirSetup = _directorySetupRepository
            .Data
            .Items
            .Where(dir => dir.Path.ContainsFilepath(filePath))
            .Aggregate(null as DirectorySetup, (aggregation, current) => aggregation == null || current.Path.Value.Length > aggregation.Path.Value.Length//maxby on pathlength
                ? current
                : aggregation)!;

        var fileSubPath = filePath.Value.Remove(0, dirSetup.Path.Value.Length);
        var tags = fileSubPath
            .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
            .Select(Tag.From)
            .ToList();

        tags.Remove(tags.Last());//remove filename
        tags.Add(dirSetup.Name.Value);//add dirSetupName

        return tags;
    }
}