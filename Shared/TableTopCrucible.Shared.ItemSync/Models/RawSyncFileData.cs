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
    internal class RawSyncFileData
    {
        private readonly IFileInfo foundFileInfo;

        public RawSyncFileData(ScannedFileData knownFile, FilePath foundFile)
        {
            this.KnownFile = knownFile;
            this.OriginalHashKey = knownFile?.HashKey;
            FoundFile = foundFile;
            this.foundFileInfo = FoundFile?.GetInfo();
            UpdateSource = GetFileState();
        }
        public ScannedFileData KnownFile { get; }
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

        public ScannedFileData GetNewFileEntity()
            => new(NewHashKey, FoundFile, foundFileInfo.LastWriteTime, KnownFile?.Id);

        public ItemUpdateHelper GetItemUpdateHelper(IEnumerable<Item> items)
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
        public Item LinkedItemAfterSync { get; }

        public ItemUpdateHelper(RawSyncFileData fileHelper, IEnumerable<Item> items)
        {
            HashAfterSync = fileHelper.NewHashKey;
            File = fileHelper.FoundFile;
            UpdateSource = fileHelper.UpdateSource;

            LinkedItemAfterSync = items.FirstOrDefault(item => item.ModelFileKey == HashAfterSync);
        }

        public Item GetItemUpdate()
        {
            // temporary solution: only handle new items
            if (LinkedItemAfterSync is null) // there is no item for the updated file
            {
                // todo: autoTagging
                return new Item(File.GetFilenameWithoutExtension().ToName(), HashAfterSync);
            }
            return null;
        }

    }
}
