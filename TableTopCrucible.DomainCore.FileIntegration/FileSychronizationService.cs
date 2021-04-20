using Microsoft.Extensions.Logging;

using ReactiveUI;

using Serilog.Events;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs;
using TableTopCrucible.Core.Jobs.Enums;
using TableTopCrucible.Core.Jobs.Managers;
using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Library.ValueTypes.IDs;
using TableTopCrucible.Data.Models.Sources;

using Version = TableTopCrucible.Data.Library.Models.ValueTypes.General.Version;

namespace TableTopCrucible.DomainCore.FileIntegration
{
    [Singleton(typeof(FileSychronizationService))]
    public interface IFileSynchronizationService
    {
        IObservable<Unit> StartSync();

    }
    internal class FileSychronizationService : IFileSynchronizationService
    {
        private readonly ILogger<FileSychronizationService> logger;
        private readonly IItemService _itemService;
        private readonly IJobService _jobManagementService;
        private readonly IFileSetupService _fileSetupService;

        int threadCount = 3;
        public FileSychronizationService(IItemService itemService, IJobService jobManagementService, IFileSetupService fileSetupService, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<FileSychronizationService>();
            _itemService = itemService;
            _jobManagementService = jobManagementService;
            _fileSetupService = fileSetupService;

        }
        // todo: second input with hashed files
        public IObservable<Unit> StartSync()
        {
            var job = this._jobManagementService.TrackJob();

            job.Title = "File synchronization";


            var readFiles_1 = job.TrackProgression("Load files", 1, 1);
            var hashFiles_2 = Enumerable.Range(0, threadCount).Select(i => job.TrackProgression($"Hash Files {i}", 1, 1)).ToArray();
            var generateItems_3 = job.TrackProgression($"Generate Items", 1, 1);
            //var changesetGenProg_4 = job.TrackProgression("Create Changesets", 1, 1);
            //var removeFileProg_5 = job.TrackProgression("Remove deleted files", 1, 1);
            //var changesetGenProg_6 = job.TrackProgression("Update changed files", 1, 1);
            //var addFilesProg_7 = job.TrackProgression("Add new files", 1, 1);
            //var addVersionsProg_8 = job.TrackProgression("create new versions", 1, 1); 
            //var addNewItems_9 = job.TrackProgression("create new items", 1, 1);


            logger.LogInformation("Sync Scheduled");

            var res = Observable.StartAsync(async () =>
                    {
                        using var scope = logger.BeginScope(nameof(StartSync) + " - pre hash");
                        logger.LogInformation("Sync Started");


                        var files = getFilePathsForDirectory(readFiles_1);
                        var hashedFiles = await hashFiles(files, hashFiles_2);
                        var items = getItems(hashedFiles, generateItems_3);
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
                    .Where(file => file.Type.IsIn(PathType.Image, PathType.Model));
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
                var filteredFiles = files.Where(file => file.Type == PathType.Model).ToArray();
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
                this.Type = FileSupportHelper.GetPathType(path);
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
            public PathType Type { get; }
            public FileHash Hash { get; private set; }
            public FileDataHashKey HashKey => FileDataHashKey.From((Hash, FileInfo.Length));
            public void CreateHash() => Hash = FileHash.Create(Path);
            public FileData GetFileData()
                => new FileData(Path, DateTime.Now, Hash, fileInfo.LastWriteTime, Directory.Id, fileInfo.Length);


        }
    }

}