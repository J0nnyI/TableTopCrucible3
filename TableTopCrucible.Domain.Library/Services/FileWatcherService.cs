﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

using DynamicData;
using DynamicData.PLinq;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.ItemSync.Services;

namespace TableTopCrucible.Domain.Library.Services
{
    internal class FileWatcherHelper : IDisposable
    {
        public DirectorySetup DirectorySetup { get; }
        private FileSystemWatcher _fileWatcher;
        private readonly CompositeDisposable _disposables = new();
        public void Dispose() => _disposables.Dispose();
        private readonly Subject<DirectorySetup> _onChange = new();
        public IObservable<DirectorySetup> OnChange => _onChange.AsObservable();
        public FileWatcherHelper(DirectorySetup directorySetup)
        {
            DirectorySetup = directorySetup;
            _fileWatcher = new FileSystemWatcher(directorySetup.Path.Value).DisposeWith(_disposables);
            _fileWatcher.BeginInit();
            _fileWatcher.EnableRaisingEvents = true;
            _fileWatcher.Created += (_, _) => _onChange.OnNext(directorySetup);
            _fileWatcher.Deleted += (_, _) => _onChange.OnNext(directorySetup);
            _fileWatcher.Changed += (_, _) => _onChange.OnNext(directorySetup);
            _fileWatcher.Renamed += (_, _) => _onChange.OnNext(directorySetup);
            _fileWatcher.EndInit();
        }
    }

    [Singleton]
    public interface IFileWatcherService
    {
        void StartSynchronization();

    }

    internal class FileWatcherService : ReactiveObject, IFileWatcherService, IDisposable
    {
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly IFileSynchronizationService _fileSynchronizationService;
        private readonly INotificationService _notificationService;
        private CompositeDisposable _syncDisposables { get; set; }
        private readonly ObservableAsPropertyHelper<bool> _syncRunning;
        public bool SyncRunning => _syncRunning.Value;
        public FileWatcherService(
            IDirectorySetupRepository directorySetupRepository,
            IFileSynchronizationService fileSynchronizationService,
            INotificationService notificationService)
        {
            _directorySetupRepository = directorySetupRepository;
            _fileSynchronizationService = fileSynchronizationService;
            _notificationService = notificationService;

            this.WhenAnyValue(srv => srv._syncDisposables)
                .Select(x => x is not null)
                .ToProperty(this, x => x.SyncRunning, out _syncRunning);
        }
        public void StartSynchronization()
        {
            if (SyncRunning)
                throw new InvalidOperationException("Synchronization is already activated");
            _syncDisposables = new();

            _directorySetupRepository
                .Data
                .Connect()
                .Transform(dir => new FileWatcherHelper(dir))
                .DisposeMany()
                .Transform(dir => dir.OnChange.StartWith(dir.DirectorySetup))
                .ToCollection()
                .Select(x => x.Merge())
                .Switch()
                .Buffer(SettingsHelper.AutoFileScanThrottle)
                .Where(buffer => buffer.Any())
                .Subscribe(changedDirs =>
                {

                    var watcher = _fileSynchronizationService.StartScan();
                    var scanStarted = watcher is not null;

                    if (!scanStarted)
                        return;

                    var distinctDirs = changedDirs.Distinct().ToArray();
                    if (distinctDirs.Length == 1)
                    {
                        _notificationService.AddNotification(
                            (Name)"Starting scan for changed Files",
                            (Description)$"File changes detected in the last {SettingsHelper.AutoFileScanThrottle} in directory '{distinctDirs.First().Name}' ({distinctDirs.First().Path})",
                            NotificationType.Info);
                    }
                    else
                    {
                        _notificationService.AddNotification(
                            (Name)"Starting Scan for changed Files",
                            (Description)(
                                $"File changes detected in the last {SettingsHelper.AutoFileScanThrottle} in directories '{distinctDirs.First()}'" + Environment.NewLine +
                                string.Join(Environment.NewLine,
                                    distinctDirs.Select(dir => $"'{dir.Name}' ({dir.Path})").ToArray())),
                            NotificationType.Info);

                    }

                }, e =>
                {

                }).DisposeWith(_syncDisposables);
        }

        public void StopSynchronization()
        {
            _syncDisposables.Dispose();
            _syncDisposables = null;
        }

        public void Dispose()
        {
            _syncRunning?.Dispose();
            _syncDisposables?.Dispose();
        }
    }
}
