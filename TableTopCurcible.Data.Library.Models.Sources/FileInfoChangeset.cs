using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.IO;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Models.Sources;
using TableTopCrucible.Domain.Models.ValueTypes;
using TableTopCrucible.WPF.Helper;

using TableTopCurcible.Data.Library.ValueTypes.IDs;

using SysFileInfo = System.IO.FileInfo;

namespace TableTopCrucible.Data.Models.Sources
{
    public class FileInfoChangeset : EntityChangesetBase<FileData, FileInfoId>, IEntityChangeset<FileData, FileInfoId>
    {
        [Reactive]
        public Uri Path { get; set; }

        [Reactive]
        public DateTime CreationTime { get; set; }

        [Reactive]
        public FileHash? FileHash { get; set; }

        [Reactive]
        public DateTime LastWriteTime { get; set; }

        [Reactive]
        public SourceDirectoryId DirectorySetupId { get; set; }

        [Reactive]
        public bool IsAccessible { get; set; }

        [Reactive]
        public long FileSize { get; set; }



        public FileInfoChangeset(SourceDirectory dirSetup, Uri relativePath) : base(null)
        {
            var localPath = new Uri(dirSetup.Path, relativePath).LocalPath;
            var fileInfo = new SysFileInfo(localPath);
            Path = relativePath;
            CreationTime = fileInfo.CreationTime;
            FileHash = Library.Models.ValueTypes.FileHash.Create(localPath);
            DirectorySetupId = dirSetup.Id;
            FileSize = fileInfo.Length;
            IsAccessible = File.Exists(localPath);
            LastWriteTime = fileInfo.LastWriteTime;

        }
        public FileInfoChangeset(FileData? origin = null) : base(origin)
        {
            if (origin.HasValue)
            {
                Path = origin.Value.Path;
                CreationTime = origin.Value.FileCreationTime;
                FileHash = origin.Value.FileHash;
                LastWriteTime = origin.Value.LastWriteTime;
                DirectorySetupId = origin.Value.DirectorySetupId;
                IsAccessible = origin.Value.IsAccessible;
                FileSize = origin.Value.FileSize;
            }
        }
        public FileInfoChangeset(SourceDirectory directorySetup, SysFileInfo fileInfo, FileHash hash, FileData? origin = null) : this(origin)
        {
            SetSysFileInfo(directorySetup, fileInfo);
            FileHash = hash;
        }
        public void SetSysFileInfo(SourceDirectory directorySetup, SysFileInfo fileInfo)
        {
            DirectorySetupId = directorySetup.Id;
            Path = fileInfo != null ? directorySetup.Path.MakeUnescapedRelativeUri(fileInfo?.FullName) : null;
            CreationTime = fileInfo?.CreationTime ?? default;
            LastWriteTime = fileInfo?.LastWriteTime ?? default;
            FileSize = fileInfo?.Length ?? default;
            IsAccessible = fileInfo != null;
        }
        public override FileData Apply()
            => new FileData(Origin.Value, Path, CreationTime, FileHash, LastWriteTime, DirectorySetupId, FileSize, IsAccessible);
        public override IEnumerable<string> GetErrors() => throw new NotImplementedException();
        public override FileData ToEntity()
            => new FileData(Path, CreationTime, FileHash, LastWriteTime, DirectorySetupId, FileSize, IsAccessible);
    }
}
