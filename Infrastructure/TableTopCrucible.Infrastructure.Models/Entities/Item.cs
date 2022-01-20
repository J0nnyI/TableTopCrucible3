using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using DynamicData;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Helper;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
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

        [JsonIgnore]
        private Name _name;
        public Name Name
        {
            get => _name;
            set => SetRequiredValue(ref _name, value);
        }

        private FileHashKey _fileKey3d;
        public FileHashKey FileKey3d
        {
            get => _fileKey3d;
            set => SetRequiredValue(ref _fileKey3d, value);
        }
        public SourceList<Tag> Tags { get; } = new();
        
    }
}