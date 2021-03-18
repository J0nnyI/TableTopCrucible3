using System;
using System.Diagnostics.Eventing.Reader;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Library.Models.ValueTypes;

using PathType = TableTopCrucible.Core.Helper.PathType;

namespace TableTopCrucible.Data.Models.Sources
{
    public struct FileData 
    {
        public FileData(
            FileData origin, string path, DateTime creationTime, FileHash? fileHash, DateTime lastWriteTime, SourceDirectoryId directorySetupId, long fileSize)
            : this(path, creationTime, fileHash, lastWriteTime, directorySetupId, fileSize, origin.Id, origin.Created)
        { }
        public FileData(
            string path, DateTime creationTime, FileHash? fileHash, DateTime lastWriteTime, SourceDirectoryId directorySetupId, long fileSize)
            : this(path, creationTime, fileHash, lastWriteTime, directorySetupId, fileSize, FileDataId.New(), DateTime.Now)
        { }
        public FileData(
            string path,
            DateTime creationTime,
            FileHash? fileHash,
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
            this.Identity = Guid.NewGuid();
            this.HashKey = null;
            this.Type = FileSupportHelper.GetPathType(Path);
            if (this.FileHash.HasValue)
                this.HashKey = new FileDataHashKey(this.FileHash.Value, this.FileSize);
        }

        public FileDataHashKey? HashKey { get; }
        public string Path { get; }
        // the time when the file (not the model) was created
        public DateTime FileCreationTime { get; }
        public FileHash? FileHash { get; }
        public DateTime LastWriteTime { get; }
        public SourceDirectoryId DirectorySetupId { get; }
        // identifies this item in this specific state
        public long FileSize { get; }
        public Guid Identity { get; }

        public FileDataId Id { get; }
        public DateTime Created { get; }
        public DateTime LastChange { get; }
        public PathType Type { get; }

        public static bool operator ==(FileData fileA, FileData fileB)
            => fileA.Identity == fileB.Identity;
        public static bool operator !=(FileData fileA, FileData fileB)
            => fileA.Identity != fileB.Identity;

        public override bool Equals(object obj) => obj is FileData info && this == info;
        public override int GetHashCode() => HashCode.Combine(this.Identity);
    }
}
