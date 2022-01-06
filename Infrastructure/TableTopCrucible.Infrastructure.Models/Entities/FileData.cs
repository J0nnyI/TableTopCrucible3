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

        public FileData(FilePath path, FileHashKey hashKey, DateTime lastWrite)
        {
            Path = path;
            HashKey = hashKey;
            LastWrite = lastWrite;
        }

        private FilePath _path;
        public FilePath Path
        {
            get => _path;
            set => SetRequiredValue(ref _path, value);
        }
        
        public FileHashKey HashKey { get; set; }

        public string HashKey_Raw // required to be public by database queries
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
            => new object[] { Id, Path, HashKey, LastWrite };
    }

    public class FileDataConfiguration : IEntityTypeConfiguration<FileData>
    {
        public static string TableName => "Files";
        public void Configure(EntityTypeBuilder<FileData> builder)
        {
            builder.ToTable(TableName);
            builder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            builder.OwnsOne(x => x.Path)
                .Property(x => x.Value)
                .HasColumnName("FileLocation")
                .IsRequired();
            builder
                .Property(x => x.LastWrite)
                .IsRequired();
            builder.Ignore(
                fileData => fileData.HashKey);
            builder.HasIndex(x => x.HashKey_Raw)
                .HasDatabaseName("FileKey3d");

            builder.HasOne(file => file.Item)
                .WithMany(item => item.Files)
                .HasForeignKey(file => file.HashKey_Raw)
                .IsRequired(false)
                .HasPrincipalKey(item => item.FileKey3d_Raw)
                .IsRequired(false);

            builder.Ignore(x => x.Id);
            builder.HasKey(x => x.Guid)
                .HasName("Id");
        }
    }
}