using System;
using System.Collections.Generic;
using DynamicData.Binding;
using TableTopCrucible.Core.DataAccess.Models;
using TableTopCrucible.Data.Library.Models.Values;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Library.Models.DataSource
{
    public class Item:IEntity<ItemId>
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
            ItemId? id = null)
            : this(id ?? ItemId.New(), name, tags, versions, DateTime.Now) { }
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

    }
}
