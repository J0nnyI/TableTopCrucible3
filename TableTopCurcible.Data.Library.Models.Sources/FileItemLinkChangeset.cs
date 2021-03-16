using System;
using System.Collections.Generic;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Domain.Models.ValueTypes;
using TableTopCrucible.Domain.Models.ValueTypes.IDs;

using TableTopCurcible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Models.Sources
{
    public class FileItemLinkChangeset : IEntityChangeset<FileItemLink, FileItemLinkId>
    {
        public ItemId ItemId { get; set; }

        public FileDataHashKey FileKey { get; set; }

        public Version Version { get; set; }
        public FileItemLink? Origin { get; set; }
        public FileDataHashKey? ThumbnailKey { get; set; }
        public FileItemLinkChangeset(FileItemLink? origin = null)
        {
            Origin = origin;
            if (origin.HasValue)
            {
                ItemId = origin.Value.ItemId;
                Version = origin.Value.Version;
                FileKey = origin.Value.FileKey;
                ThumbnailKey = origin.Value.ThumbnailKey;
            }
        }
        public FileItemLink Apply() => new FileItemLink(Origin.Value, ItemId, FileKey, ThumbnailKey, Version);
        public IEnumerable<string> GetErrors() => throw new NotImplementedException();
        public FileItemLink ToEntity() => new FileItemLink(ItemId, FileKey, ThumbnailKey, Version);
    }
}
