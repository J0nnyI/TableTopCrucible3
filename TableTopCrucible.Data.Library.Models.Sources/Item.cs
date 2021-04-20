using DynamicData;
using DynamicData.Binding;

using System;
using System.Collections.Generic;

using TableTopCrucible.Core.Data;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

using Version = TableTopCrucible.Data.Library.Models.ValueTypes.General.Version;

namespace TableTopCrucible.Data.Models.Sources
{
    public class Item
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
            : this(id??ItemId.New(), name, tags, versions, DateTime.Now) { }
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
