using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
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
        private readonly IScannedFileRepository _fileRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IProgressTrackingService _progressTrackingService;

        public FileSynchronizationService(
            IDirectorySetupRepository directorySetupRepository,
            IScannedFileRepository fileRepository,
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

                        var dbgItems = new List<IEnumerable<RawSyncFileData>>();

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
                            .ForAll(item =>
                            {
                                item.CreateNewHashKey();
                                hashingTracker.Increment((ProgressIncrement)item.FoundFile.Value.Length);
                                updatePipeline.OnNext(item);
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
                    throw;
                }
            }, RxApp.TaskpoolScheduler);

            return totalProgress;
        }

        private IEnumerable<RawSyncFileData> startScanForDirectory(DirectorySetupEntity directory)
        {
            var foundFiles = directory.Path.GetFiles(FileType.Image, FileType.Model).ToArray();
            var knownFiles = _fileRepository.Data;
            return foundFiles.FullJoin(
                knownFiles,
                foundFile => foundFile.Value,
                knownFile => knownFile.FileLocation.Value,
                found => new RawSyncFileData(null, found),
                known => new RawSyncFileData(known, null),
                (found, known) => new RawSyncFileData(known, found));
        }

        private FileSyncListGrouping getFileGroups(IQueryable<DirectorySetupEntity> directorySetups)
        {
            return new FileSyncListGrouping(directorySetups
                .SelectMany(directory => startScanForDirectory(directory)));
        }


        private void _handleChangedFiles(RawSyncFileData[] files)
        {
            var toAdd = files.Where(x => x.UpdateSource == FileUpdateSource.New).Select(file => file.GetNewEntity());
            var toUpdate = files.Where(x => x.UpdateSource == FileUpdateSource.Updated)
                .Select(file => file.GetNewEntity());
            _fileRepository.AddRange(toAdd);
        }
    }
}