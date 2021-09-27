using MoreLinq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Input;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Models;
using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Shared.ItemSync.Models;

namespace TableTopCrucible.Shared.ItemSync.Services
{
    /// <summary>
    /// Synchronizes the master file list with the files in the directory
    /// </summary>
    [Singleton(typeof(FileSynchronizationService))]
    public interface IFileSynchronizationService
    {
        ITrackingViewer StartScan();
        ICommand StartScanCommand { get; }
    }
    public class FileSynchronizationService : IFileSynchronizationService
    {
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly IScannedFileRepository _fileRepository;
        private readonly IProgressTrackingService _progressTrackingService;
        [Reactive] public bool ScanRunning { get; private set; } = false;

        public FileSynchronizationService(
            IDirectorySetupRepository directorySetupRepository,
            IScannedFileRepository fileRepository,
            IProgressTrackingService progressTrackingService)
        {
            _directorySetupRepository = directorySetupRepository;
            _fileRepository = fileRepository;
            _progressTrackingService = progressTrackingService;

            this.StartScanCommand = ReactiveCommand.Create(
                StartScan,
                this.WhenAnyValue(s => s.ScanRunning)
                    .Select(x => !x));
        }

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
            var stepTracker = totalProgress.AddSingle((Name)"Total", (TrackingTarget)1, (TrackingWeight)1);
            var hashingTracker = totalProgress.AddSingle((Name)"hashing", null, (TrackingWeight)10);
            var updateTracker = totalProgress.AddSingle((Name)"update", null, (TrackingWeight)3);

            totalProgress
                .OnDone()
                .Take(1)
                .Subscribe(_ =>
            {
                ScanRunning = false;
            });

            Observable.Start(() =>
            {
                try
                {
                    var files = getFileGroups(
                        _directorySetupRepository
                        .Data
                        .Items);

                    stepTracker.Increment();

                    // remove deleted files
                    _fileRepository.Delete(files.DeletedFiles.Select(file => file.KnownFile.Id));

                    // prepare hash process
                    var filesToHash = files.NewFiles
                        .Concat(files.UpdatedFiles)
                        .ToArray();

                    if (filesToHash.Length > 0)
                    {
                        hashingTracker.SetTarget((TrackingTarget)filesToHash.Length);
                        updateTracker.SetTarget((TrackingTarget)filesToHash.Length);

                        var updatePipeline = new Subject<RawSyncFileData>();

                        var dbgItems = new List<IEnumerable<RawSyncFileData>>();

                        updatePipeline
                            .ObserveOn(RxApp.TaskpoolScheduler)
                            .Buffer(SettingsHelper.PipelineBufferTime)
                            .TakeUntil(updateTracker.OnDone())
                            .Subscribe(items =>
                            {
                                _handleChangedFiles(items);
                                updateTracker.Increment((ProgressIncrement)items.Count);
                            });

                        // hash items and do the rest in parallel
                        filesToHash
                            .AsParallel()
                            .WithDegreeOfParallelism(SettingsHelper.Threadcount)
                            .ForAll(item =>
                            {
                                item.CreateNewHashKey();
                                hashingTracker.Increment();
                                updatePipeline.OnNext(item);
                                Thread.Sleep(TimeSpan.FromSeconds(2));
                            });
                    }
                    else
                    {
                        hashingTracker.OnCompleted();
                        updateTracker.OnCompleted();
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

        private IEnumerable<RawSyncFileData> startScanForDirectory(DirectorySetup directory)
        {
            var foundFiles = directory.Path.GetFiles(FileType.Image, FileType.Model).ToArray();
            var knownFiles = _fileRepository.Data.Items;
            return foundFiles.FullJoin(
                knownFiles,
                foundFile => foundFile.Value,
                knownFile => knownFile.FileLocation.Value,
                found => new RawSyncFileData(null, found),
                known => new RawSyncFileData(known, null),
                (found, known) => new RawSyncFileData(known, found));

        }

        private FileSyncListGrouping getFileGroups(IEnumerable<DirectorySetup> directorySetups)
        {
            return new(directorySetups
                .SelectMany(dir => startScanForDirectory(dir)));
        }


        private void _handleChangedFiles(IEnumerable<RawSyncFileData> files)
        {
            _fileRepository.AddOrUpdate(files.Select(file => file.GetNewFileEntity()));
        }
    }
}
