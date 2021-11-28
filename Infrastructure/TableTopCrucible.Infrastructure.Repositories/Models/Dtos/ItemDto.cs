using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.Configuration.Annotations;

using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;
using ValueOf;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Dtos
{
    [DataContract]
    public class ItemDto : IEntityDto<ItemId, Item>
    {
        public string Name { get; set; }
        public byte[] ModelFileHash { get; set; }
        public long ModelFileSize { get; set; }
        public Guid Id { get; set; }

        public Item ToEntity()
        {
            return new Item(
                (Name) Name,
                FileHashKey.From((FileHash.From(ModelFileHash), FileSize.From(ModelFileSize))),
                ItemId.From(Id)
            );
        }

        public void Initialize(Item sourceEntity)
        {
            Name = sourceEntity.Name.Value;
            ModelFileHash = sourceEntity.ModelFileKey.Hash.Value;
            ModelFileSize = sourceEntity.ModelFileKey.FileSize.Value;
            Id = sourceEntity.Id.Value;
        }
    }
}
