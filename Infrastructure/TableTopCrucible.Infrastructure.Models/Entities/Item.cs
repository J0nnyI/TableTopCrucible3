using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            set => SetRequiredValue(ref _fileKey3d, value, nameof(FileKey3d), nameof(FileKey3dRaw));
        }
        public string FileKey3dRaw // required to be public by database queries
        {
            get => _fileKey3d.Value;
            set => SetRequiredValue(ref _fileKey3d, (FileHashKey)value, nameof(FileKey3d), nameof(FileKey3dRaw));
        }

        public ObservableCollection<Tag> Tags { get; } = new();
        internal string RawTags
        {
            get => SerializedStringList.From(Tags.Select(tag => tag.Value)).Value;
            set
            {
                var newTags = SerializedStringList.From(value).Deserialize().Select(tag => (Tag)tag);
                Tags.AddRange(newTags.Except(Tags));
            }
        }

        private readonly ObservableCollection<ImageData> _images = new();
        public ObservableCollection<ImageData> Images
        {
            get => _images;
            set {
                _images.Add(value.Except(_images));
                _images.Remove(_images.Except(value));
            }
        }

        public void AddTags(IEnumerable<Tag> tags)
           => Tags.Add(tags.Except(Tags));
    }

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public static string TableName => "Items";

        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable(TableName);
            builder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            //indices
            builder.Ignore(x => x.Id);
            builder.HasKey(x => x.Guid)
                .HasName("Id");

            builder.Ignore(x => x.FileKey3d);
            builder.HasIndex(x => x.FileKey3dRaw)
                .IsUnique();

            //properties
            builder.OwnsOne(x => x.Name)
                .Property(x => x.Value)
                .HasColumnName("Name")
                .IsRequired();
            
            builder.Property(x => x.RawTags)
                .HasColumnName("Tags");
            builder.Ignore(x => x.Tags);

        }
    }
}