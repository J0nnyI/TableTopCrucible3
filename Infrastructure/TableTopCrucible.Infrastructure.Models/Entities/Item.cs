#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using DynamicData;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Item : DataEntity<ItemId>
    {

        public Item()
        {
        }

        public Item(Name name, FileHashKey fileHashKey, IEnumerable<Tag> tags = null)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            FileKey3d = fileHashKey ?? throw new NullReferenceException(nameof(fileHashKey));
            if (tags is not null)
                Tags.AddRange(tags);
        }

        private Name _name;
        [JsonProperty]
        public Name Name
        {
            get => _name;
            set => SetRequiredValue(ref _name, value);
        }

        private FileHashKey _fileKey3d;
        [JsonProperty]
        public FileHashKey FileKey3d
        {
            get => _fileKey3d;
            set => SetRequiredValue(ref _fileKey3d, value);
        }

        [JsonProperty]
        public SourceList<Tag> Tags { get; } = new();

    }
}