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
            builder.HasIndex(img => img.HashKeyRaw)
                .IsUnique();

            builder.Ignore(x => x.Id);
            builder.HasKey(x => x.Guid)
                .HasName("Id");

            //properties
            builder.OwnsOne(x => x.Name)
                .Property(x => x.Value)
                .HasColumnName("Name")
                .IsRequired();
        }
    }
}
