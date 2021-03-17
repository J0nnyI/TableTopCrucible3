using System;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Library.Models.ValueTypes;

namespace TableTopCrucible.Data.Models.Sources
{
    public struct ItemVersion
    {
        public ItemVersion(
            ItemVersion origin,
            ItemId itemId,
            FileDataHashKey fileKey,
            FileDataHashKey? thumbnailKey,
            Version version)
            : this(
            origin.Id,
            itemId == default ? origin.ItemId : itemId,
            fileKey == default ? origin.FileKey : fileKey,
            thumbnailKey == default ? origin.ThumbnailKey : thumbnailKey,
            version == default ? origin.Version : version,
            DateTime.Now)
        { }


        public ItemVersion(
            ItemId itemId,
            FileDataHashKey fileKey,
            FileDataHashKey? thumbnailKey,
            Version version)
            : this(
            (ItemVersionId)Guid.NewGuid(),
            itemId,
            fileKey,
            thumbnailKey,
            version,
            DateTime.Now)
        { }


        public ItemVersion(
            ItemVersionId id,
            ItemId itemId,
            FileDataHashKey fileKey,
            FileDataHashKey? thumbnailKey,
            Version version,
            DateTime created,
            DateTime? lastChange = null)
        {
            Id = id;
            ItemId = itemId;
            FileKey = fileKey;
            Version = version;
            Created = created;
            ThumbnailKey = thumbnailKey;
            LastChange = lastChange ?? DateTime.Now;
        }

        public ItemVersionId Id { get; }
        public ItemId ItemId { get; }

        public FileDataHashKey FileKey { get; }
        public FileDataHashKey? ThumbnailKey { get; }
        public Version Version { get; }

        public DateTime Created { get; }
        public DateTime LastChange { get; }
    }
}
