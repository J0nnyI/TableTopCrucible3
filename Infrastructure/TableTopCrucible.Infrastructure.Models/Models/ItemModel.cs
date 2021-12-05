using System.Collections.Generic;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.Models
{
    public class ItemModel:IDataModel<ItemId>
    {
        public ItemId Id { get; }
        public Name Name { get; }
        public FileHashKey ModelFileKey { get; }
        public IReadOnlyCollection<Tag> Tags { get; init; }
        public ItemModel(Name name, FileHashKey modelFileKey, ItemId id = null) 
            : base()
        {
            Id = id ?? ItemId.New();
            Name = name;
            ModelFileKey = modelFileKey;
        }
    }
}
