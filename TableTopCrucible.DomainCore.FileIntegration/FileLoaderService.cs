using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Models.Sources;

using Version = TableTopCrucible.Data.Library.Models.ValueTypes.General.Version;

namespace TableTopCrucible.DomainCore.FileIntegration
{
    [Singleton((typeof(FileLoaderService)))]
    public interface IFileLoaderService
    {
        void StartSync();

    }
    internal class FileLoaderService : IFileLoaderService
    {
        private readonly IItemService _itemService;
        private readonly IJobManagementService _jobManagementService;
        private readonly IFileSetupService _fileSetupService;


        public FileLoaderService(IItemService itemService, IJobManagementService jobManagementService, IFileSetupService fileSetupService)
        {
            _itemService = itemService;
            _jobManagementService = jobManagementService;
            _fileSetupService = fileSetupService;
        }
        public void StartSync()
        {
            this._jobManagementService
                .Start<IEnumerable<RawFileData>>(GetFilePathsForDirectory)
                .Then<IEnumerable<RawFileData>>(hashFiles)
                .Then<IEnumerable<Item>>(getItems)
                .Then<object>(writeItems);
        }
        private void GetFilePathsForDirectory(IJobHandler<IEnumerable<RawFileData>> handler)
        {
            handler.Result.Subscribe(x =>
            {

            });
            var dirWithFiles = _fileSetupService
            .Directories
            .Items
            .SelectMany(dir =>
                Directory.GetFiles(dir.Path, "*", SearchOption.AllDirectories)
                    .Select(file => new RawFileData(dir, file))
            )
            .Where(file => file.Type.IsIn(PathType.Image, PathType.Model));
            handler.Complete(dirWithFiles);
        }
        private void hashFiles(IJobHandler<IEnumerable<RawFileData>> handler, IEnumerable<RawFileData> files)
        {
            using var prog = handler.TrackProgression(files.Count(), "hashing files", "not started");
            foreach (var file in files)
            {
                prog.Details = file.Path;
                prog.CurrentProgress++;
                file.CreateHash();
            }
            prog.Details = "done";
            handler.Complete(files);
        }
        private void getItems(IJobHandler<IEnumerable<Item>> handler, IEnumerable<RawFileData> files)
        {
            var res = files.Where(file => file.Type == PathType.Model)
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
            handler.Complete(res);
        }
        private void writeItems(IJobHandler<object> handler, IEnumerable<Item> items)
        {
            this._itemService.AddOrUpdate(items);
            handler.Complete(null);
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