using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DynamicData;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public sealed class Item : DataEntity<ItemId>
    {
        private Name _name;

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

        public Name Name
        {
            get => _name;
            set => SetRequiredValue(ref _name, value);
        }

        private FileHashKey _fileKey3d;
        public FileHashKey FileKey3d
        {
            get => _fileKey3d;
            set
            {
                SetRequiredValue(ref _fileKey3d, value);
                SetRequiredValue(ref _fileKey3d_Raw, value.ToString(), nameof(FileKey3d_Raw));
            }
        }
        private string _fileKey3d_Raw;
        public string FileKey3d_Raw => _fileKey3d_Raw;// required to be public by database queries

        public ObservableCollection<Tag> Tags { get; } = new();
        public ObservableCollection<FileData> Files { get; } = new();

        internal string RawTags
        {
            get => SerializedStringList.From(Tags.Select(tag => tag.Value)).Value;
            set
            {
                var newTags = SerializedStringList.From(value).Deserialize().Select(tag => (Tag)tag);
                Tags.AddRange(newTags.Except(Tags));
            }
        }

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { Id, Name, FileKey3d, Tags };
    }

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public static string TableName => "Items";

        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable(TableName);
            builder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            builder.OwnsOne(x => x.Name)
                .Property(x => x.Value)
                .HasColumnName("Name")
                .IsRequired();
            builder.OwnsOne(x => x.Id)
                .Property(x => x.Value)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(fileData => fileData.FileKey3d_Raw);
            builder.HasIndex(x => x.FileKey3d_Raw);
            
            builder.Property(x => x.RawTags);
            
            builder.Ignore(x => x.Tags);
            builder.Ignore(x => x.FileKey3d);

            builder.Ignore(x => x.Id);
            builder.HasKey(x => x.Guid)
                .HasName("Id");
        }
    }
}