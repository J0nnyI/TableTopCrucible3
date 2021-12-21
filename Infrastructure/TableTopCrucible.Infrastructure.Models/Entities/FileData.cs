using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public sealed class FileData : DataEntity<ScannedFileDataId>
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
        
        private FileHashKey _hashKey;
        public FileHashKey HashKey
        {
            get => _hashKey;
            set
            {
                SetRequiredValue(ref _hashKey, value);
                SetRequiredValue(ref _hashKey_Raw, value.ToString(), nameof(HashKey_Raw));
            }
        }
        private string _hashKey_Raw;
        internal string HashKey_Raw => _hashKey_Raw;

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
        public void Configure(EntityTypeBuilder<FileData> fileBuilder)
        {
            fileBuilder.ToTable("Files");
            fileBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            fileBuilder.OwnsOne(x => x.FileLocation)
                .Property(x => x.Value)
                .HasColumnName("FileLocation")
                .IsRequired();
            fileBuilder.OwnsOne(x => x.Id)
                .Property(x => x.Guid)
                .HasColumnName("Id")
                .IsRequired();
            fileBuilder
                .Property(x => x.LastWrite)
                .IsRequired();
            fileBuilder.OwnsOne(
                fileData => fileData.HashKey,
                hashKeyBuilder =>
                {
                    hashKeyBuilder.OwnsOne(
                        hashKey => hashKey.Hash,
                        hashBuilder => hashBuilder
                            .Property(hash => hash.Value)
                            .HasColumnName("HashKey_Hash")
                            .IsRequired()
                        );
                    hashKeyBuilder.OwnsOne(
                        hashKey => hashKey.FileSize,
                        fileSizeBuilder => fileSizeBuilder
                            .Property(fileSize => fileSize.Value)
                            .HasColumnName("HashKey_FileSize")
                            .IsRequired()
                        );
                    hashKeyBuilder.Ignore(x => x.ValueA);
                    hashKeyBuilder.Ignore(x => x.ValueB);
                });

            fileBuilder.HasIndex(x => x.HashKey_Raw);

            fileBuilder.HasOne(file => file.Item)
                .WithMany(item => item.Files)
                .HasForeignKey(file => file.HashKey_Raw)
                .HasPrincipalKey(item => item.FileKey3d_Raw);

            fileBuilder.HasKey(x => x.Guid);
        }
    }
}