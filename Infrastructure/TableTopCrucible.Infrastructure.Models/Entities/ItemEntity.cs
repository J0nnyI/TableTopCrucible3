using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;
using TableTopCrucible.Infrastructure.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class ItemEntity:IDataEntity<ItemId, ItemModel>
    {
        public string Name { get; set; }
        public byte[] ModelFileHash { get; set; }
        public long ModelFileSize { get; set; }
        public Guid Id { get; set; }
        public string Tags { get; set; } = "[]";

        public ItemModel ToModel()
        {
            return new (
                (Name) Name,
                FileHashKey.From((FileHash.From(ModelFileHash), FileSize.From(ModelFileSize))),
                JsonSerializer.Deserialize<string[]>(Tags)!.Select(Tag.From).ToArray(),
                ItemId.From(Id)
            );
        }
    }
}
