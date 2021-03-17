using System;
using System.Collections.Generic;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;

namespace TableTopCrucible.Data.Models.Sources
{
    public struct Item : IEntity<ItemId>
    {
        public ItemId Id { get; }
        public ItemName Name { get; }
        public IEnumerable<Tag> Tags { get; }

        public DateTime Created { get; }
        public DateTime LastChange { get; }

        public Item(ItemName name, IEnumerable<Tag> tags = null)
            : this((ItemId)Guid.NewGuid(), name, tags, DateTime.Now) { }
        public Item(Item origin, ItemName name, IEnumerable<Tag> tags)
            : this(origin.Id, name, tags, origin.Created) { }

        public Item(ItemId id, ItemName name, IEnumerable<Tag> tags, DateTime created, DateTime? lastChange = null)
        {
            Id = id;
            Name = name;
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
            Created = created;
            LastChange = lastChange ?? DateTime.Now;
        }

        public override string ToString() => $"Tile {Id} ({Name})";
        public override bool Equals(object obj) => obj is Item item && this.Id == item.Id && this.LastChange == item.LastChange;
        public override int GetHashCode() => HashCode.Combine(LastChange, Id);

    }
}
