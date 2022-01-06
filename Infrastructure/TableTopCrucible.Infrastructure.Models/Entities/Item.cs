using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        internal string FileKey3d_Raw => _fileKey3d_Raw;
        
        public ObservableCollection<Tag> Tags { get; } = new();
        public ObservableCollection<FileData> Files { get; } = new();

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { Id, Name, FileKey3d, Tags };
    }

    public class ItemConfiguration:IEntityTypeConfiguration<Item>
    {
        public static string TableName => "Items";

        public void Configure(EntityTypeBuilder<Item> itemBuilder)
        {
            itemBuilder.ToTable(TableName);
            itemBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            itemBuilder.OwnsOne(x => x.Name)
                .Property(x=>x.Value)
                .HasColumnName("Name")
                .IsRequired();
            itemBuilder.OwnsOne(x => x.Id)
                .Property(x=>x.Value)
                .HasColumnName("Id")
                .IsRequired();
            itemBuilder.OwnsOne(
                fileData => fileData.FileKey3d);
            itemBuilder.HasIndex(x => x.FileKey3d_Raw);
            itemBuilder.OwnsMany(x => x.Tags);
            
            itemBuilder.HasKey(x => x.Guid);
        }
    }
}