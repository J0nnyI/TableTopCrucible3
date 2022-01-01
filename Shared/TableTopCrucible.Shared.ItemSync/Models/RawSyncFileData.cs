using System;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Shared.ItemSync.Models
{
    internal class RawSyncFileData
    {
        private readonly IFileInfo foundFileInfo;

        public RawSyncFileData(FileData knownFile, FilePath foundFile)
        {
            KnownFile = knownFile;
            OriginalHashKey = knownFile?.HashKey;
            FoundFile = foundFile;
            foundFileInfo = FoundFile?.GetInfo();
            UpdateSource = GetFileState();
        }

        public FileData KnownFile { get; }
        public FilePath FoundFile { get; }

        public FileHashKey NewHashKey { get; private set; }
        public FileHashKey OriginalHashKey { get; }

        public FileUpdateSource UpdateSource { get; }


        public FileHashKey CreateNewHashKey(HashAlgorithm algorithm = null)
        {
            if (NewHashKey != null)
                return NewHashKey;

            var hash = FileHashKey.Create(FoundFile, algorithm ?? new SHA512Managed());
            NewHashKey = hash;
            return hash;
        }

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

        /// <summary>
        /// </summary>
        /// <returns>the updated old or a new entity depending on the type of change</returns>
        public FileData GetNewEntity()
        {
            if (KnownFile == null)
                return new FileData(FoundFile, NewHashKey, foundFileInfo.LastWriteTime);
            KnownFile.HashKey = NewHashKey;
            KnownFile.FileLocation = FoundFile;
            KnownFile.LastWrite = foundFileInfo.LastWriteTime;
            return KnownFile;
        }

        public ItemUpdateHelper GetItemUpdateHelper(IQueryable<Item> items)
            => new(this, items);
    }

    internal class ItemUpdateHelper
    {
        public ItemUpdateHelper(RawSyncFileData fileHelper, IQueryable<Item> items)
        {
            HashAfterSync = fileHelper.NewHashKey;
            File = fileHelper.FoundFile;
            UpdateSource = fileHelper.UpdateSource;

            LinkedItemAfterSync = items.Single(item => item.FileKey3d == HashAfterSync);
        }

        public FileHashKey HashAfterSync { get; }
        public FileUpdateSource UpdateSource { get; }
        public FilePath File { get; }

        /// item which is linked to
        /// <see cref="HashAfterSync" />
        public Item LinkedItemAfterSync { get; }

        public Item GetItemUpdate()
        {
            // temporary solution: only handle new items
            if (LinkedItemAfterSync is null) // there is no item for the updated file
                // todo: autoTagging
                return new Item(File.GetFilenameWithoutExtension().ToName(), HashAfterSync);

            LinkedItemAfterSync.FileKey3d = HashAfterSync;
            return LinkedItemAfterSync;
        }
    }
}