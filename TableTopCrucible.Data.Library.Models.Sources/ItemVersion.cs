using DynamicData.Binding;

using System;
using System.Collections.Generic;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

using Version = TableTopCrucible.Data.Library.Models.ValueTypes.General.Version;

namespace TableTopCrucible.Data.Models.Sources
{
    public class ItemVersion
    {
        public ItemVersion(
            ItemVersion origin,
            ItemId itemId,
            FileDataHashKey fileKey,
            Version version,
            IEnumerable<FileData> files = null)
            : this(
            origin.Id,
            itemId == default ? origin.ItemId : itemId,
            fileKey == default ? origin.FileKey : fileKey,
            version == default ? origin.Version : version,
            DateTime.Now,
            null,
            files)
        { }


        public ItemVersion(
            ItemId itemId,
            FileDataHashKey fileKey,
            Version version,
            IEnumerable<FileData> files = null)
            : this(
            ItemVersionId.New(),
            itemId,
            fileKey,
            version,
            DateTime.Now,
            null,
            files)
        { }


        public ItemVersion(
            ItemVersionId id,
            ItemId itemId,
            FileDataHashKey fileKey,
            Version version,
            DateTime created,
            DateTime? lastChange = null,
            IEnumerable<FileData> files = null)
        {
            Id = id;
            ItemId = itemId;
            FileKey = fileKey;
            Version = version;
            Created = created;
            LastChange = lastChange ?? DateTime.Now;
            if (files != null)
                this.Files.AddRange(files);
        }

        public ItemVersionId Id { get; }
        public ItemId ItemId { get; }

        public FileDataHashKey FileKey { get; }
        public Version Version { get; }

        public DateTime Created { get; }
        public DateTime LastChange { get; }
        public ObservableCollectionExtended<FileData> Files { get; } = new ObservableCollectionExtended<FileData>();
    }
}
