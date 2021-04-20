using System;
using System.Diagnostics.Eventing.Reader;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

using PathType = TableTopCrucible.Core.Helper.PathType;

namespace TableTopCrucible.Data.Models.Sources
{
    public class FileData 
    {
        public FileData(
            FileData origin, FilePath path, DateTime creationTime, FileHash fileHash, DateTime lastWriteTime, SourceDirectoryId directorySetupId, long fileSize)
            : this(path, creationTime, fileHash, lastWriteTime, directorySetupId, fileSize, origin.Id, origin.Created)
        { }
        public FileData(
            FilePath path, DateTime creationTime, FileHash fileHash, DateTime lastWriteTime, SourceDirectoryId directorySetupId, long fileSize)
            : this(path, creationTime, fileHash, lastWriteTime, directorySetupId, fileSize, FileDataId.New(), DateTime.Now)
        { }
        public FileData(
            FilePath path,
            DateTime creationTime,
            FileHash fileHash,
            DateTime lastWriteTime,
            SourceDirectoryId directorySetupId,
            long fileSize,
            FileDataId id,
            DateTime created,
            DateTime? lastChange = null)
        {
            this.Path = path;
            this.FileCreationTime = creationTime;
            this.FileHash = fileHash;
            this.LastWriteTime = lastWriteTime;
            this.DirectorySetupId = directorySetupId;
            this.FileSize = fileSize;
            this.Id = id;
            this.Created = created;
            this.LastChange = lastChange ?? DateTime.Now;
            this.HashKey = null;
            this.Type = FileSupportHelper.GetPathType(Path);
            if (this.FileHash != null)
                this.HashKey = FileDataHashKey.From((this.FileHash, this.FileSize));
        }

        public FileDataHashKey HashKey { get; }
        public string Path { get; }
        // the time when the file (not the model) was created
        public DateTime FileCreationTime { get; }
        public FileHash FileHash { get; }
        public DateTime LastWriteTime { get; }
        public SourceDirectoryId DirectorySetupId { get; }
        // identifies this item in this specific state
        public long FileSize { get; }

        public FileDataId Id { get; }
        public DateTime Created { get; }
        public DateTime LastChange { get; }
        public PathType Type { get; }


    }
}
