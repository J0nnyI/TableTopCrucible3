using System;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Domain.Models.ValueTypes;

using TableTopCrucible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Models.Sources
{
    public struct FileData : IEntity<FileInfoId>
    {
        public FileData(
            FileData origin, Uri path, DateTime creationTime, FileHash? fileHash, DateTime lastWriteTime, SourceDirectoryId directorySetupId, long fileSize,
            bool isAccessible)
            : this(path, creationTime, fileHash, lastWriteTime, directorySetupId, fileSize, isAccessible, origin.Id, origin.Created)
        { }
        public FileData(
            Uri path, DateTime creationTime, FileHash? fileHash, DateTime lastWriteTime, SourceDirectoryId directorySetupId, long fileSize,
            bool isAccessible)
            : this(path, creationTime, fileHash, lastWriteTime, directorySetupId, fileSize, isAccessible, (FileInfoId)Guid.NewGuid(), DateTime.Now)
        { }
        public FileData(
            Uri path,
            DateTime creationTime,
            FileHash? fileHash,
            DateTime lastWriteTime,
            SourceDirectoryId directorySetupId,
            long fileSize,
            bool isAccessible,
            FileInfoId id,
            DateTime created,
            DateTime? lastChange = null)
        {
            this.Path = path;
            this.FileCreationTime = creationTime;
            this.FileHash = fileHash;
            this.LastWriteTime = lastWriteTime;
            this.DirectorySetupId = directorySetupId;
            this.FileSize = fileSize;
            this.IsAccessible = isAccessible;
            this.Id = id;
            this.Created = created;
            this.LastChange = lastChange ?? DateTime.Now;
            this.Identity = Guid.NewGuid();
            this.HashKey = null;
            if (this.FileHash.HasValue)
                this.HashKey = new FileDataHashKey(this.FileHash.Value, this.FileSize);
        }

        public FileDataHashKey? HashKey { get; }
        public Uri Path { get; }
        // the time when the file (not the model) was created
        public DateTime FileCreationTime { get; }
        public FileHash? FileHash { get; }
        public DateTime LastWriteTime { get; }
        public SourceDirectoryId DirectorySetupId { get; }
        public bool IsAccessible { get; }
        // identifies this item in this specific state
        public long FileSize { get; }
        public Guid Identity { get; }

        public FileInfoId Id { get; }
        public DateTime Created { get; }
        public DateTime LastChange { get; }

        public static bool operator ==(FileData fileA, FileData fileB)
            => fileA.Identity == fileB.Identity;
        public static bool operator !=(FileData fileA, FileData fileB)
            => fileA.Identity != fileB.Identity;

        public override bool Equals(object obj) => obj is FileData info && this == info;
        public override int GetHashCode() => HashCode.Combine(this.Identity);
    }
}
