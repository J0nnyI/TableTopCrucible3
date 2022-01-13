using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

using Microsoft.EntityFrameworkCore;

using MoreLinq;

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

namespace TableTopCrucible.Shared.ItemSync.Services
{
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

        public FileSynchronizationService(
            IDirectorySetupRepository directorySetupRepository,
            IFileRepository fileRepository,
            IItemRepository itemRepository,
            IProgressTrackingService progressTrackingService)
        {
            _directorySetupRepository = directorySetupRepository;
            _fileRepository = fileRepository;
            _itemRepository = itemRepository;
            _progressTrackingService = progressTrackingService;

            StartScanCommand = ReactiveCommand.Create(
                StartScan,
                this.WhenAnyValue(s => s.ScanRunning)
                    .Select(x => !x));
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

            totalProgress
                .OnDone()
                .Take(1)
                .Subscribe(_ => { ScanRunning = false; });

            Observable.Start(() =>
            {
                try
                {
                    var files = getFileGroups(
                        _directorySetupRepository
                            .Data);

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

                        var updatePipeline = new Subject<RawSyncFileData>();

                        updatePipeline
                            .ObserveOn(RxApp.TaskpoolScheduler)
                            .Buffer(SettingsHelper.PipelineBufferTime)
                            .TakeUntil(updateTracker.OnDone())
                            .Subscribe(items =>
                            {
                                _handleChangedFiles(items.ToArray());
                                updateTracker.Increment((ProgressIncrement)items.Count);
                            });

                        // hash items and do the rest in parallel
                        filesToHash
                            .AsParallel()
                            .WithDegreeOfParallelism(SettingsHelper.ThreadCount)
                            .ForAll(fileData =>
                            {
                                fileData.CreateNewHashKey();
                                hashingTracker.Increment((ProgressIncrement)fileData.FoundFile.Value.Length);
                                lock (updatePipeline)
                                {
                                    updatePipeline.OnNext(fileData);
                                }
                            });
                    }
                    else
                    {
                        hashingTracker.Complete();
                        updateTracker.Complete();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Debugger.Break();
                    stepTracker.Complete();
                    hashingTracker.Complete();
                    updateTracker.Complete();
                    throw;
                }
            }, RxApp.TaskpoolScheduler);

            return totalProgress;
        }

        private IEnumerable<RawSyncFileData> startScanForDirectory(DirectorySetup directory)
        {
            var foundFiles = directory.Path.GetFiles(FileType.Image, FileType.Model).ToArray();
            var knownFiles = _fileRepository.Data.Where(file=>file.PathRaw.ToLower().StartsWith(directory.Path.Value.ToLower()));
            return foundFiles.FullJoin(
                knownFiles,
                foundFile => foundFile.Value.ToLower(),
                knownFile => knownFile.Path.Value.ToLower(),
                found => new RawSyncFileData(null, found),
                known => new RawSyncFileData(known, null),
                (found, known) => new RawSyncFileData(known, found));
        }

        private FileSyncListGrouping getFileGroups(IQueryable<DirectorySetup> directorySetups)
        {
            var dirs = _directorySetupRepository.Data.AsEnumerable().Select(dir => dir.Path);
            var foundFiles = directorySetups
                .AsEnumerable()
                .SelectMany(startScanForDirectory)
                .ToArray();

            var filesOfUnregisteredDirs = _fileRepository
                .Data
                .AsEnumerable()
                .Where(file => !dirs.Any(dir =>  file.Path.Value.ToLower().StartsWith(dir.Value.ToLower())))
                .Select(file => new RawSyncFileData(file, null))
                .ToArray();

            var groupings = new FileSyncListGrouping(foundFiles.Concat(filesOfUnregisteredDirs));

            return groupings;
        }

        private object _itemUpdateLocker = new();
        private void _handleChangedFiles(RawSyncFileData[] files)
        {
            var filesToAdd = files.Where(x => x.UpdateSource == FileUpdateSource.New).Select(file => file.GetNewEntity()).ToArray();
            _fileRepository.AddRange(filesToAdd);
            lock (_itemUpdateLocker)
            {
                var modelFiles = filesToAdd
                    .Where(file => file.Path.GetExtension().IsModel())
                    .ToArray();
                var itemsToAdd = modelFiles
                    .DistinctBy(x => x.HashKeyRaw)
                    .Where(file =>
                        !_itemRepository.Data.Any(item => item.FileKey3dRaw == file.HashKeyRaw)) // all linked files have to be checked manually in case there was no file with a given hash found before
                    .Select(file => new Item(
                            file.Path.GetFilenameWithoutExtension().ToName(),
                            file.HashKey
                        )
                    ).ToArray();

                modelFiles.ForEach(modelFile =>
                {
                    var tags = GetTagsByPath(modelFile.Path);
                    var item = 
                        itemsToAdd.SingleOrDefault(item=>item.FileKey3d == modelFile.HashKey) ??
                        _itemRepository.Data.Single(item => item.FileKey3dRaw == modelFile.HashKeyRaw);
                    item.AddTags(tags);
                });

                _itemRepository.AddRange(itemsToAdd);
            }
        }

        private IEnumerable<Tag> GetTagsByPath(FilePath filePath)
        {
            var dir = _directorySetupRepository
                .Data
                .ToArray()
                .Where(dir => dir.Path.ContainsFilepath(filePath))
                .Aggregate(string.Empty, (path, dir) => dir.Path.Value.Length > path.Length ? dir.Path.Value : path);

            var fileSubPath =filePath.Value.Remove(0,dir.Length);
            return fileSubPath.Split(Path.DirectorySeparatorChar).Select(Tag.From);
        }
    }
}