using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using DynamicData;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public sealed class ItemEntity : DataEntity<ItemId>
    {
        private Name _name;

        public Name Name
        {
            get => _name;
            set => RaiseAndSetRequiredIfChanged(ref _name, value);
        }

        private FileHashKey _fileHashKey;

        public FileHashKey FileHashKey
        {
            get => _fileHashKey;
            set => RaiseAndSetRequiredIfChanged(ref _fileHashKey, value);
        }

        public ObservableCollection<Tag> Tags { get; } = new();

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { Id, Name, FileHashKey, Tags };

        public ItemEntity()
        {
        }

        public ItemEntity(Name name, FileHashKey fileHashKey, IEnumerable<Tag> tags = null)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            FileHashKey = fileHashKey ?? throw new NullReferenceException(nameof(fileHashKey));
            if (tags is not null)
                Tags.AddRange(tags);
        }
    }
}