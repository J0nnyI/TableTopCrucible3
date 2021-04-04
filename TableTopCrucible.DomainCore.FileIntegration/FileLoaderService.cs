using ReactiveUI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs;
using TableTopCrucible.Core.Jobs.Enums;
using TableTopCrucible.Core.Jobs.Managers;
using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Models.Sources;

using Version = TableTopCrucible.Data.Library.Models.ValueTypes.General.Version;

namespace TableTopCrucible.DomainCore.FileIntegration
{
    [Singleton(typeof(FileLoaderService))]
    public interface IFileLoaderService
    {
        void StartSync();

    }
    internal class FileLoaderService : IFileLoaderService
    {
        private readonly IItemService _itemService;
        private readonly IJobService _jobManagementService;
        private readonly IFileSetupService _fileSetupService;

        int threadCount = 3;
        public FileLoaderService(IItemService itemService, IJobService jobManagementService, IFileSetupService fileSetupService)
        {
            _itemService = itemService;
            _jobManagementService = jobManagementService;
            _fileSetupService = fileSetupService;
        }
        // todo: second input with hashed files
        public void StartSync()
        {
            var job = this._jobManagementService.TrackJob();
            job.Title = "File synchronization";
            var changesetGenProg_4 = job.TrackProgression("Create Changesets", 1, 1);
            var removeFileProg_5 = job.TrackProgression("Remove deleted files", 1, 1);
            var changesetGenProg_6 = job.TrackProgression("Update changed files", 1, 1);
            var addFilesProg_7 = job.TrackProgression("Add new files", 1, 1);
            var addVersionsProg_8 = job.TrackProgression("create new versions", 1, 1);
            var addNewItems_9 = job.TrackProgression("create new items", 1, 1);

            var _1 = getFilePathsForDirectory(job.TrackProgression("Load files", 1, 1));
            var _2 = hashFiles(_1, Enumerable.Range(1, threadCount)
                    .Select(i => job.TrackProgression($"Hash untracked Files {i}", 1, 5/threadCount)))

                .Subscribe(x=>
                {
                });
        }

        private IObservable<IEnumerable<RawFileData>> getFilePathsForDirectory(IProgressionHandler progress)
        {
            return Observable.Start(() =>
            {
                progress.State = JobState.InProgress;
                try
                {
                    var res = _fileSetupService
                    .Directories
                    .Items
                    .SelectMany(dir =>
                        Directory.GetFiles(dir.Path, "*", SearchOption.AllDirectories)
                            .Select(file => new RawFileData(dir, file))
                    )
                    .Where(file => file.Type.IsIn(PathType.Image, PathType.Model));
                    progress.State = JobState.Done;
                    return res;
                }
                catch (Exception ex)
                {
                    progress.State = JobState.Failed;
                    progress.Details = ex.ToString();
                    throw ex;
                }
            }, RxApp.TaskpoolScheduler);
        }
        private IObservable<IEnumerable<RawFileData>> hashFiles(IObservable<IEnumerable<RawFileData>> fileChanges, IEnumerable<IProgressionHandler> progress)
        {
            return fileChanges
                   .Select(files => files
                       .SplitEvenly(progress.Count())
                       .Select(fileGroup => Observable.Start(() =>
                       { 
                           var prog = progress.ElementAt(fileGroup.Key);
                           prog.WhenAnyValue(x => x.Current).Subscribe(x =>
                           {

                           });
                           prog.State = JobState.InProgress;
                           try
                           {
                               prog.Target = fileGroup.Count();
                               fileGroup.ToList().ForEach(file =>
                               {
                                   prog.Details = file.Path;
                                   prog.Current++;
                                   file.CreateHash();
                               });
                               prog.State = JobState.Done;
                               return fileGroup;
                           }
                           catch (Exception ex)
                           {
                               prog.State = JobState.Failed;
                               prog.Details = $"failed {ex}";
                               throw ex;
                           }
                       }, RxApp.TaskpoolScheduler)
                       )
                       .CombineLatest(values => values.SelectMany(x => x))
                   )
                   .Switch();
        }
        private IEnumerable<Item> getItems(IJobHandler job, IEnumerable<RawFileData> files)
        {
            return files.Where(file => file.Type == PathType.Model)
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
        }
        private void writeItems(IJobHandler handler, IEnumerable<Item> items)
        {
            this._itemService.AddOrUpdate(items);

        }

        private struct RawFileData
        {
            public RawFileData(SourceDirectory directory, string path)
            {
                Directory = directory;
                this.Path = path;
                this.Type = FileSupportHelper.GetPathType(path);
                this.fileInfo = null;
                this.Hash = null;
            }

            public SourceDirectory Directory { get; }
            public string Path { get; }
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
            public FileHash? Hash { get; private set; }
            public FileDataHashKey HashKey => new FileDataHashKey(Hash ?? throw new InvalidOperationException("hash not created"), FileInfo.Length);
            public void CreateHash() => Hash = FileHash.Create(Path);
            public FileData GetFileData()
            => new FileData(Path, DateTime.Now, Hash, fileInfo.LastWriteTime, Directory.Id, fileInfo.Length);


        }
    }

}