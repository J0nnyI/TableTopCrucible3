using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Entities
{
    public class Item
    {
        public ItemId Id { get; }
        public Name Name { get; }
        public FileHashKey ModelFileKey { get; }
        public IReadOnlyCollection<Tag> Tags { get; init; }
        public Item(Name name, FileHashKey modelFileKey, ItemId id = null) 
            : base()
        {
            Id = id ?? ItemId.New();
            Name = name;
            ModelFileKey = modelFileKey;
        }
    }
}
