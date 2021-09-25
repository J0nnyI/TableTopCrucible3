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
        private IFileInfo foundFileInfo;

        public RawSyncFileData(ScannedFileData knownFile, FilePath foundFile)
        {
            this.KnownFile = knownFile;
            FoundFile = foundFile;
            State = GetFileState();

            this.foundFileInfo = FoundFile.GetInfo();
        }
        public ScannedFileData KnownFile { get; }
        public FilePath FoundFile { get; }

        private BehaviorSubject<FileHashKey> _newHashKey { get; } = new(null);
        public IObservable<FileHashKey> NewHashKeyChanges => _newHashKey.WhereNotNull();

        public FileHashKey CreateNewHashkey(HashAlgorithm algorithm = null)
        {
            if (this._newHashKey.Value != null)
                return _newHashKey.Value;

            var hash = FileHashKey.From(
                FileHash.Create(FoundFile, algorithm ?? new SHA512Managed()),
                FileSize.From(foundFileInfo.Length));
            this._newHashKey.OnNext(hash);
            this._newHashKey.OnCompleted();
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

        public ScannedFileData GetNewEntity()
            => new(_newHashKey.Value, FoundFile, foundFileInfo.LastWriteTime, KnownFile?.Id);
    }

    internal class FileSyncListGrouping
    {
        public IEnumerable<RawSyncFileData> NewFiles { get; }
        public IEnumerable<RawSyncFileData> UpdatedFiles { get; }
        public IEnumerable<RawSyncFileData> DeletedFiles { get; }

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
        }

        public void UpdateFileHashes()
        {
            using var algorithm = new SHA512Managed();

            NewFiles.Concat(UpdatedFiles).ToList().ForEach(file =>
            {
                file.CreateNewHashkey(algorithm);
            });
        }

        public IObservable<RawSyncFileData> GetFileHashUpdateFeed()
        {
            return NewFiles
                .Concat(UpdatedFiles)
                .ToArray()
                .ToObservable()
                .Do(data => data.CreateNewHashkey());
        }
    }
}
