using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public sealed class FileData : DataEntity<FileDataId>
    {
        public FileData()
        {
        }

        public FileData(FilePath fileLocation, FileHashKey hashKey, DateTime lastWrite)
        {
            FileLocation = fileLocation;
            HashKey = hashKey;
            LastWrite = lastWrite;
        }

        private FilePath _fileLocation;
        public FilePath FileLocation
        {
            get => _fileLocation;
            set => SetRequiredValue(ref _fileLocation, value);
        }
        
        public FileHashKey HashKey { get; set; }

        internal string HashKey_Raw
        {
            get => HashKey.Value;
            set => HashKey = FileHashKey.From(value);
        }

        private DateTime _lastWrite;
        public DateTime LastWrite
        {
            get => _lastWrite;
            set => SetRequiredValue(ref _lastWrite, value);
        }
        

        [Reactive]
        public Item Item { get; set; }

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { Id, FileLocation, HashKey, LastWrite };
    }

    public class FileDataConfiguration : IEntityTypeConfiguration<FileData>
    {
        public static string TableName => "Files";
        public void Configure(EntityTypeBuilder<FileData> builder)
        {
            builder.ToTable(TableName);
            builder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            builder.OwnsOne(x => x.FileLocation)
                .Property(x => x.Value)
                .HasColumnName("FileLocation")
                .IsRequired();
            builder
                .Property(x => x.LastWrite)
                .IsRequired();
            builder.Ignore(
                fileData => fileData.HashKey);
            builder.HasIndex(x => x.HashKey_Raw);

            builder.HasOne(file => file.Item)
                .WithMany(item => item.Files)
                .HasForeignKey(file => file.HashKey_Raw)
                .HasPrincipalKey(item => item.FileKey3d_Raw);

            builder.Ignore(x => x.Id);
            builder.HasKey(x => x.Guid)
                .HasName("Id");
        }
    }
}