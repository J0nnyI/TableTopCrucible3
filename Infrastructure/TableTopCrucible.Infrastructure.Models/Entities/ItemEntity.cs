using System;
using System.Runtime.Serialization;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class ItemEntity:IDataEntity
    {
        public string Name { get; set; }
        public byte[] ModelFileHash { get; set; }
        public long ModelFileSize { get; set; }
        public Guid Id { get; set; }

        public ItemModel ToEntity()
        {
            return new ItemModel(
                (Name) Name,
                FileHashKey.From((FileHash.From(ModelFileHash), FileSize.From(ModelFileSize))),
                ItemId.From(Id)
            );
        }

        public void Initialize(ItemModel sourceEntity)
        {
            Name = sourceEntity.Name.Value;
            ModelFileHash = sourceEntity.ModelFileKey.Hash.Value;
            ModelFileSize = sourceEntity.ModelFileKey.FileSize.Value;
            Id = sourceEntity.Id.Value;
        }
    }
}
