using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public sealed class ItemEntity : DataEntity<ItemId>
    {
        private FileHashKey _fileHashKey;
        private Name _name;

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

        public Name Name
        {
            get => _name;
            set => RaiseAndSetRequiredIfChanged(ref _name, value);
        }

        public FileHashKey FileHashKey
        {
            get => _fileHashKey;
            set => RaiseAndSetRequiredIfChanged(ref _fileHashKey, value);
        }

        public ObservableCollection<Tag> Tags { get; } = new();

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { Id, Name, FileHashKey, Tags };
    }
}