using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;
using TableTopCrucible.Infrastructure.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.ChangeSets
{
    public class ItemChangeSet : IDataChangeSet<ItemId, ItemModel, ItemEntity>
    {
        public ItemId Id { get; set; }
        public Name Name { get; set; }
        public FileHashKey ModelFileKey { get; set; }
        public List<Tag> Tags { get; } = new();

        public ItemEntity ToEntity()
        {
            var entity = new ItemEntity();
            UpdateEntity(entity);
            return entity;
        }

        public ItemModel ToModel() =>
            new(Name, ModelFileKey, Tags, Id);

        public void UpdateEntity(ItemEntity entity)
        {
            entity.Id = Id.Guid;
            entity.ModelFileHash = ModelFileKey.Hash.Value;
            entity.ModelFileSize = ModelFileKey.FileSize.Value;
            entity.Name = Name.Value;
            entity.Tags = Tags.Select(tag => tag.Value).ToJson().Value;
        }

        public ItemChangeSet()
        {

        }
        public ItemChangeSet(Name name, FileHashKey modelFileKey, IEnumerable<Tag> Tags, ItemId id = null)
            : base()
        {
            Id = id ?? ItemId.New();
            Name = name;
            ModelFileKey = modelFileKey;
        }
    }
}
