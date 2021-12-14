using ReactiveUI;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Shared.ItemSync.Models
{
    internal class RawSyncFileData
    {
        private readonly IFileInfo foundFileInfo;

        public RawSyncFileData(ScannedFileDataEntity knownFile, FilePath foundFile)
        {
            this.KnownFile = knownFile;
            this.OriginalHashKey = knownFile?.HashKey;
            FoundFile = foundFile;
            this.foundFileInfo = FoundFile?.GetInfo();
            UpdateSource = GetFileState();
        }
        public ScannedFileDataEntity KnownFile { get; }
        public FilePath FoundFile { get; }

        public FileHashKey NewHashKey { get; private set; }
        public FileHashKey OriginalHashKey { get; private set; }


        public FileHashKey CreateNewHashKey(HashAlgorithm algorithm = null)
        {
            if (this.NewHashKey != null)
                return NewHashKey;

            var hash = FileHashKey.From(
                FileHash.Create(FoundFile, algorithm ?? new SHA512Managed()),
                FileSize.From(foundFileInfo.Length));
            NewHashKey = hash;
            return hash;
        }

        public FileUpdateSource UpdateSource { get; }

        private FileUpdateSource GetFileState()
        {
            if (FoundFile == null && KnownFile == null)
                throw new ArgumentNullException(nameof(FoundFile), "at least one file must be set");
            if (FoundFile != null && KnownFile == null)
                return FileUpdateSource.New;
            if (FoundFile == null && KnownFile != null)
                return FileUpdateSource.Deleted;
            if (foundFileInfo.LastWriteTime == KnownFile!.LastWrite)
                return FileUpdateSource.Unchanged;

            return FileUpdateSource.Updated;
        }

        public void GetNewEntity()
        {
            if (KnownFile == null)
                KnownFile = new();
            KnownFile.HashKey = NewHashKey;
            KnownFile.FileLocation = FoundFile;
            KnownFile.LastWrite = foundFileInfo.LastWriteTime;
        }

        public ItemUpdateHelper GetItemUpdateHelper(IEnumerable<ItemEntity> items)
        {
            return new(this, items);
        }
    }

    internal class ItemUpdateHelper
    {
        public FileHashKey HashAfterSync { get; }
        public FileUpdateSource UpdateSource { get; }
        public FilePath File { get; }

        /// item which is linked to <see cref="HashAfterSync"/>
        public ItemEntity LinkedItemAfterSync { get; }

        public ItemUpdateHelper(RawSyncFileData fileHelper, IEnumerable<ItemEntity> items)
        {
            HashAfterSync = fileHelper.NewHashKey;
            File = fileHelper.FoundFile;
            UpdateSource = fileHelper.UpdateSource;

            LinkedItemAfterSync = items.FirstOrDefault(item => item.ModelFileKey == HashAfterSync);
        }

        public ItemChangeSet GetItemUpdate()
        {
            // temporary solution: only handle new items
            if (LinkedItemAfterSync is null) // there is no item for the updated file
            {
                // todo: autoTagging
                return new (File.GetFilenameWithoutExtension().ToName(), HashAfterSync, Enumerable.Empty<Tag>());
            }
            return null;
        }

    }
}
