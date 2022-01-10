using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class ImageData:DataEntity<ImageDataId>
    {
        [Reactive]
        public Name Name { get; set; }

        private FileHashKey _hashKey;
        public FileHashKey HashKey
        {
            get => _hashKey;
            set => SetRequiredValue(ref _hashKey, value, nameof(HashKey), nameof(HashKeyRaw));
        }
        public string HashKeyRaw // required to be public by database queries
        {
            get => _hashKey.Value;
            set => SetRequiredValue(ref _hashKey, (FileHashKey)value, nameof(HashKey), nameof(HashKeyRaw));
        }

        private ItemId _itemId;
        public ItemId ItemId
        {
            get => _itemId;
            set => SetValue(ref _itemId, value, nameof(ItemId), nameof(ItemIdRaw));
        }
        public Guid ItemIdRaw
        {
            get => _itemId.Value;
            set => SetValue(ref _itemId,ItemId.From(value), nameof(ItemId), nameof(ItemIdRaw));
        }

        public ImageData()
        {
                
        }

        [Reactive]
        public Item Item { get; set; }

        public ImageData(Name name, FileHashKey hashKey)
        {
            _hashKey = hashKey;
            Name = name;
        }
    }

    public class ImageDataConfiguration : IEntityTypeConfiguration<ImageData>
    {
        public static string TableName => "Images";
        public void Configure(EntityTypeBuilder<ImageData> builder)
        {
            builder.ToTable(TableName);
            builder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            //indices
            builder.Ignore(img => img.HashKey);
            builder.HasIndex(img => img.HashKeyRaw);

            builder.Ignore(x => x.Id);
            builder.HasKey(x => x.Guid)
                .HasName("Id");

            //foreign keys
            builder.Ignore(image => image.ItemId);
            builder.HasOne(image => image.Item)
                .WithMany(item => item.Images)
                .HasForeignKey(image => image.ItemIdRaw)
                .HasConstraintName("ItemId")
                .IsRequired(false);

            //properties
            builder.OwnsOne(x => x.Name)
                .Property(x => x.Value)
                .HasColumnName("Name")
                .IsRequired();
        }
    }
}
