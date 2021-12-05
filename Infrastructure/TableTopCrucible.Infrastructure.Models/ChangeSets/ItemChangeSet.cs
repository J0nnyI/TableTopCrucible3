using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.ChangeSets
{
    public class ItemChangeSet
    {
        public ItemId Id { get; }
        public Name Name { get; }
        public FileHashKey ModelFileKey { get; }
        public List<Tag> Tags { get; init; } = new();
    }
}
