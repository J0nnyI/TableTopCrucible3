using System;
using System.Collections.Generic;
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
        void StartScan();
        ICommand StartScanCommand { get; }
    }
    public class FileSynchronizationService : IFileSynchronizationService
    {
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly IScannedFileRepository _fileRepository;
        [Reactive] public bool ScanRunning { get; private set; } = false;

        public FileSynchronizationService(
            IDirectorySetupRepository directorySetupRepository,
            IScannedFileRepository fileRepository)
        {
            _directorySetupRepository = directorySetupRepository;
            _fileRepository = fileRepository;

            this.StartScanCommand = ReactiveCommand.Create(
                StartScan,
                this.WhenAnyValue(s => s.ScanRunning)
                    .Select(x => !x));
        }

        public void StartScan()
        {
            this.ScanRunning = true;

            var fileGroups = getFileGroups(_directorySetupRepository.Data.Items);
            fileGroups.UpdateFileHashes();

            Subject<Unit> hashingDone = new();
            Subject<Unit> deletingDone = new();

            Observable.CombineLatest(
                Observable.Start(() =>
                {
                    fileGroups.GetFileHashUpdateFeed()
                        .Buffer(new TimeSpan(0, 0, 0, 10))
                        .Do(hashedFile =>
                        {
                            _fileRepository.AddOrUpdate(hashedFile.Select(file => file.GetNewEntity()));
                        });
                }, RxApp.TaskpoolScheduler),
                Observable.Start(() =>
                {
                    _fileRepository.Delete(fileGroups.DeletedFiles.Select(file => file.KnownFile.Id));
                }, RxApp.TaskpoolScheduler)
            ).Subscribe(_ => { }, () =>
            {
                this.ScanRunning = false;
            });



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
            return new (directorySetups
                .SelectMany(dir => startScanForDirectory(dir)));
        }

    }
}
