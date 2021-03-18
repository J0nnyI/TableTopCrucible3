using DynamicData;
using DynamicData.Binding;

using System;
using System.Collections.Generic;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;

using Version = TableTopCrucible.Data.Library.Models.ValueTypes.General.Version;

namespace TableTopCrucible.Data.Models.Sources
{
    public class Item : IEntity<ItemId>
    {
        public ItemId Id { get; }
        public ItemName Name { get; }
        public IEnumerable<Tag> Tags { get; }

        public DateTime Created { get; }
        public DateTime LastChange { get; }

        public ObservableCollectionExtended<ItemVersion> Versions { get; } = new ObservableCollectionExtended<ItemVersion>();

        public Item(
            ItemName name,
            IEnumerable<Tag> tags = null,
            IEnumerable<ItemVersion> versions = null,
            ItemId? id=null)
            : this(id??(ItemId)Guid.NewGuid(), name, tags, versions, DateTime.Now) { }
        public Item(
            Item origin,
            ItemName name,
            IEnumerable<Tag> tags,
            IEnumerable<ItemVersion> versions = null)
            : this(origin.Id, name, tags, versions, origin.Created) { }

        public Item(
            ItemId id,
            ItemName name,
            IEnumerable<Tag> tags,
            IEnumerable<ItemVersion> versions,
            DateTime created,
            DateTime? lastChange = null)
        {
            Id = id;
            Name = name;
            Tags = tags ?? new Tag[0];
            Created = created;
            LastChange = lastChange ?? DateTime.Now;
            if (versions != null)
                this.Versions.AddRange(versions);
        }

        public override string ToString() => $"Tile {Id} ({Name})";
        public override bool Equals(object obj) => obj is Item item && this.Id == item.Id && this.LastChange == item.LastChange;
        public override int GetHashCode() => HashCode.Combine(LastChange, Id);

    }
}
