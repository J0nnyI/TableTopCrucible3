﻿using Microsoft.Extensions.Logging;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Enums;
using TableTopCrucible.Core.Jobs.Managers;
using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Data.Library.DataTransfer.Services;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Library.ValueTypes.IDs;
using TableTopCrucible.Data.Models.Sources;

using Version = TableTopCrucible.Data.Library.Models.ValueTypes.General.Version;

namespace TableTopCrucible.DomainCore.FileIntegration
{
    [Singleton(typeof(FileSynchronizationService))]
    public interface IFileSynchronizationService
    {
        IObservable<Unit> StartSync();

    }
    internal class FileSynchronizationService : IFileSynchronizationService
    {
        private readonly ILogger<FileSynchronizationService> logger;
        private readonly IItemService _itemService;
        private readonly IJobService _jobManagementService;
        private readonly IFileSetupService _fileSetupService;
        private readonly IFileDataStorageService _fileDataStorageService;
        int threadCount = 3;
        public FileSynchronizationService(
            IItemService itemService,
            IJobService jobManagementService,
            IFileSetupService fileSetupService,
            ILoggerFactory loggerFactory,
            IFileDataStorageService fileDataStorageService)
        {
            logger = loggerFactory.CreateLogger<FileSynchronizationService>();
            _itemService = itemService;
            _jobManagementService = jobManagementService;
            _fileSetupService = fileSetupService;
            _fileDataStorageService = fileDataStorageService;
        }
        // todo: second input with hashed files
        public IObservable<Unit> StartSync()
        {
            var job = this._jobManagementService.TrackJob();

            job.Title = "File synchronization";


            var readFiles_1 = job.TrackProgression("Load files", 1, 1);
            var hashFiles_2 = Enumerable.Range(0, threadCount).Select(i => job.TrackProgression($"Hash Files {i}", 1, 1)).ToArray();
            var generateItems_3_1 = job.TrackProgression($"Generate Items", 1, 1);
            var writeFileMasterList_3_2 = job.TrackProgression($"Write File Masterlist", 1, 1);


            logger.LogInformation("Sync Scheduled");

            var res = Observable.StartAsync(async () =>
                    {
                        using var scope = logger.BeginScope(nameof(StartSync) + " - pre hash");
                        logger.LogInformation("Sync Started");


                        var files = getFilePathsForDirectory(readFiles_1);
                        var hashedFiles = await hashFiles(files, hashFiles_2);

                        Observable.Start(() =>
                        {
                            try
                            {
                                _fileDataStorageService.WriteMasterFileList(hashedFiles.Select(file => file.GetFileData()));
                            }
                            catch (Exception ex)
                            {
                                writeFileMasterList_3_2.Current = writeFileMasterList_3_2.Target = 1;
                                writeFileMasterList_3_2.Details = $"writing failed: {ex}";
                                logger.LogError(ex, "could not write file master list");
                            }
                        }, RxApp.TaskpoolScheduler).Take(1).Subscribe();

                        var items = getItems(hashedFiles, generateItems_3_1);
                        writeItems(items);
                        logger.LogInformation("{0} items have been updated. There is now a total of {1} items", items.Count(), this._itemService.GetCache().Count);

                    }, RxApp.TaskpoolScheduler)
                .Replay(1)
                .Take(1);
            res.Subscribe(
                onNext: _ =>
                {
                    logger.LogInformation("file sync completed");
                }, onError: err =>
                  {
                      logger.LogError("file sync failed");
                  });
            return res;
        }

        private IEnumerable<RawFileData> getFilePathsForDirectory(IProgressionHandler progress)
        {
            logger.LogTrace("reading local files");
            progress.State = JobState.InProgress;
            try
            {
                var res = _fileSetupService
                    .Directories
                    .Items
                    .SelectMany(dir =>
                        Directory.GetFiles(dir.Path, "*", SearchOption.AllDirectories)
                            .Select(file => new RawFileData(dir, FilePath.From(file)))
                    )
                    .Where(file => file.Type.IsIn(FileType.Image, FileType.Model));
                progress.State = JobState.Done;
                progress.Current++;
                logger.LogTrace("found {0} files", res.Count());
                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "files could not be read.");
                progress.State = JobState.Failed;
                progress.Details = ex.ToString();
                throw ex;
            }
        }
        private IObservable<IEnumerable<RawFileData>> hashFiles(IEnumerable<RawFileData> files, IEnumerable<IProgressionHandler> progress)
        {
            logger.LogTrace("starting hashing {0} files.", files.Count());
            return files
                .SplitEvenly(progress.Count())
                .Select(fileGroup => Observable.Start(() =>
                {
                    using var scope = logger.BeginScope("hashing {0}", fileGroup.Key);
                    logger.LogTrace("thread {0} started", fileGroup.Key);

                    var prog = progress.ElementAt(fileGroup.Key);
                    prog.WhenAnyValue(x => x.Current).Subscribe(x =>
                    {
                        logger.LogTrace("updating current to {0}", x);
                    });
                    prog.State = JobState.InProgress;
                    try
                    {
                        prog.Target = fileGroup.Count();
                        fileGroup.ToList().ForEach(file =>
                        {
                            prog.Details = file.Path;
                            prog.Current++;
                            Thread.Sleep(5000);
                            logger.LogTrace("hashing file {0}", file.Path);
                            file.CreateHash();
                        });
                        prog.State = JobState.Done;
                        return fileGroup;
                    }
                    catch (Exception ex)
                    {
                        prog.State = JobState.Failed;
                        prog.Details = $"failed {ex}";
                        logger.LogError("failed to hash file", ex);
                        throw ex;
                    }
                }, RxApp.TaskpoolScheduler
                ))
                .CombineLatest(threadResult => threadResult.SelectMany(files => files));
        }
        private IEnumerable<Item> getItems(IEnumerable<RawFileData> files, IProgressionHandler progress)
        {
            try
            {
                var filteredFiles = files.Where(file => file.Type == FileType.Model).ToArray();
                var hashes = filteredFiles.Select(x => x.Hash).ToArray();
                var fileCount = filteredFiles.Count();
                logger.LogTrace("creating {0} items: {1}", fileCount, hashes);
                var res = filteredFiles
                    .GroupBy(fileEx => fileEx.HashKey)
                    .Select(fileGroup =>
                    {
                        var itemId = ItemId.New();
                        return new Item(
                            new ItemName(Path.GetFileNameWithoutExtension(fileGroup.First().Path)),
                            null,
                            new ItemVersion(
                                itemId,
                                fileGroup.Key,
                                new Version(1, 0, 0),
                                fileGroup.Select(file => file.GetFileData())
                            ).AsArray(),
                            itemId);
                    }).ToArray();
                progress.Current++;
                logger.LogInformation("created {0} items with a total of {1} Versions", res.Count(), res.SelectMany(item => item.Versions).Count());
                return res;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "item-creation failed");
                return null;
            }
        }
        private void writeItems(IEnumerable<Item> items)
        {
            this._itemService.AddOrUpdate(items);

        }

        private class RawFileData
        {
            public RawFileData(SourceDirectory directory, FilePath path)
            {
                Directory = directory;
                this.Path = path;
                this.Type = path.GetExtension().GetFileType();
                this.fileInfo = null;
                this.Hash = null;
            }

            public SourceDirectory Directory { get; }
            public FilePath Path { get; }
            private FileInfo fileInfo;
            public FileInfo FileInfo
            {
                get
                {
                    if (fileInfo == null)
                        this.fileInfo = new FileInfo(Path);
                    return fileInfo;
                }
            }
            public FileType Type { get; }
            public FileHash Hash { get; private set; }
            public FileHashKey HashKey => FileHashKey.From((Hash, FileSize.From(FileInfo.Length)));
            public void CreateHash() => Hash = FileHash.Create(Path);
            public FileData GetFileData()
                => new FileData(FileInfo, Hash);
        }
    }

}