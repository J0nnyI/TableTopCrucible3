using System;
using System.Collections.Generic;
using System.DirectoryServices;
using MoreLinq;

using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
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
        IObservable<Unit> StartScan();
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

        public IObservable<Unit> StartScan()
        {
            this.ScanRunning = true;

            var mainTracker = _progressTrackingService.CreateNewCompositeTracker("File Synchronization");

            using var prepTracker = mainTracker.AddSingle((Name)"Preparation", (TrackingTarget)1);
            var deleteTracker = mainTracker.AddSingle((Name)"Delete Tracker", (TrackingTarget)1);
            var updateTracker = mainTracker.AddSingle((Name) "update Tracker", (TrackingTarget) 1, (TrackingWeight) 10);

            var fileGroups = getFileGroups(_directorySetupRepository.Data.Items);
            fileGroups.UpdateFileHashes();
            prepTracker.Increment();
            prepTracker.Dispose();

            Observable.CombineLatest(
                Observable.Start(() =>
                {
                    fileGroups.GetFileHashUpdateFeed()
                        .Buffer(new TimeSpan(0, 0, 0, 10))
                        .Do(hashedFiles =>
                        {
                            _fileRepository.AddOrUpdate(hashedFiles.Select(file => file.GetNewEntity()));
                            updateTracker.Increment();
                        });
                }, RxApp.TaskpoolScheduler),
                Observable.Start(() =>
                {
                    _fileRepository.Delete(fileGroups.DeletedFiles.Select(file => file.KnownFile.Id));
                    deleteTracker.Increment();
                    deleteTracker.Dispose();
                }, RxApp.TaskpoolScheduler)
            )
                .Subscribe(_ => { }, () =>
            {
                this.ScanRunning = false;
            });

            return Observable.Never<Unit>();

        }

        public ICommand StartScanCommand { get; }

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

    }
}
