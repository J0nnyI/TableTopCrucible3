using MoreLinq.Extensions;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;

namespace TableTopCrucible.Shared.ItemSync.Models
{
    internal enum FileState
    {
        New,
        Deleted,
        Updated,
        Unchanged
    }
    internal class RawSyncFileData
    {
        private readonly IFileInfo foundFileInfo;

        public RawSyncFileData(ScannedFileData knownFile, FilePath foundFile)
        {
            this.KnownFile = knownFile;
            FoundFile = foundFile;
            this.foundFileInfo = FoundFile?.GetInfo();
            State = GetFileState();
        }
        public ScannedFileData KnownFile { get; }
        public FilePath FoundFile { get; }

        public FileHashKey NewHashKey { get; private set; }

        public FileHashKey CreateNewHashKey(HashAlgorithm algorithm = null)
        {
            if (this.NewHashKey!= null)
                return NewHashKey;

            var hash = FileHashKey.From(
                FileHash.Create(FoundFile, algorithm ?? new SHA512Managed()),
                FileSize.From(foundFileInfo.Length));
            NewHashKey = hash;
            return hash;
        }

        public FileState State { get; }

        private FileState GetFileState()
        {
            if (FoundFile == null && KnownFile == null)
                throw new ArgumentNullException(nameof(FoundFile), "at least one file must be set");
            if (FoundFile != null && KnownFile == null)
                return FileState.New;
            if (FoundFile == null && KnownFile != null)
                return FileState.Deleted;
            if (foundFileInfo.LastWriteTime == KnownFile.LastWrite)
                return FileState.Unchanged;

            return FileState.Updated;
        }

        public ScannedFileData GetNewFileEntity()
            => new(NewHashKey, FoundFile, foundFileInfo.LastWriteTime, KnownFile?.Id);
    }

    internal class FileSyncListGrouping
    {
        public IEnumerable<RawSyncFileData> NewFiles { get; }
        public IEnumerable<RawSyncFileData> UpdatedFiles { get; }
        public IEnumerable<RawSyncFileData> DeletedFiles { get; }
        public IEnumerable<RawSyncFileData> UnchangedFiles { get; }

        public FileSyncListGrouping(IEnumerable<RawSyncFileData> files)
        {

            var groups =
                files
                    .GroupBy(file => file.State)
                    .ToArray();

            NewFiles = groups.FirstOrDefault(g => g.Key == FileState.New)
                ?.DistinctBy(dir => dir.FoundFile)
                ?.ToArray() ?? Array.Empty<RawSyncFileData>();

            DeletedFiles = groups.FirstOrDefault(g => g.Key == FileState.Deleted)
                ?.ToArray() ?? Array.Empty<RawSyncFileData>();

            UpdatedFiles = groups.FirstOrDefault(g => g.Key == FileState.Updated)
                ?.DistinctBy(dir => dir.FoundFile)
                ?.ToArray() ?? Array.Empty<RawSyncFileData>();

            UnchangedFiles = groups.FirstOrDefault(g => g.Key == FileState.Unchanged)
                ?.DistinctBy(dir => dir.FoundFile)
                ?.ToArray() ?? Array.Empty<RawSyncFileData>();
        }
    }
}
