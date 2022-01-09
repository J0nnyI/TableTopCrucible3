using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
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


        private DateTime _lastWrite;
        public DateTime LastWrite
        {
            get => _lastWrite;
            set => SetRequiredValue(ref _lastWrite, value);
        }
    }
    
    public class FileDataConfiguration : IEntityTypeConfiguration<FileData>
    {
        public static string ImageTableName => "Files";
        public void Configure(EntityTypeBuilder<FileData> builder)
        {
            builder.ToTable(ImageTableName);
            builder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            //Indices
            builder.Ignore(x => x.Id);
            builder.HasKey(x => x.Guid)
                .HasName("Id");

            builder.Ignore(fileData => fileData.HashKey);
            builder.HasIndex(x => x.HashKeyRaw)
                .HasDatabaseName("HashKey");

            //properties
            builder.OwnsOne(x => x.Path)
                .Property(x => x.Value)
                .HasColumnName("FileLocation")
                .IsRequired();
            builder
                .Property(x => x.LastWrite)
                .IsRequired();


        }
    }
}