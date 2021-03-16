using System;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Domain.Models.ValueTypes;
using TableTopCrucible.Domain.Models.ValueTypes.IDs;

using TableTopCurcible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Models.Sources
{
    public struct FileItemLink : IEntity<FileItemLinkId>
    {
        public FileItemLink(
            FileItemLink origin,
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


        public FileItemLink(
            ItemId itemId,
            FileDataHashKey fileKey,
            FileDataHashKey? thumbnailKey,
            Version version)
            : this(
            (FileItemLinkId)Guid.NewGuid(),
            itemId,
            fileKey,
            thumbnailKey,
            version,
            DateTime.Now)
        { }


        public FileItemLink(
            FileItemLinkId id,
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

        public FileItemLinkId Id { get; }
        public ItemId ItemId { get; }

        public FileDataHashKey FileKey { get; }
        public FileDataHashKey? ThumbnailKey { get; }
        public Version Version { get; }

        public DateTime Created { get; }
        public DateTime LastChange { get; }
    }
}
