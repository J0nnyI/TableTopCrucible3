using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class DirectorySetup : DataEntity<DirectorySetupId>
    {
        public DirectorySetup()
        {
        }
        public DirectorySetup(DirectoryPath path)
        {
            Path = path;
            Name = path.GetDirectoryName().ToName();
        }

        private Name _name;
        public Name Name
        {
            get => _name;
            set => SetRequiredValue(ref _name, value);
        }

        private DirectoryPath _path;
        public DirectoryPath Path
        {
            get => _path;
            set => SetRequiredValue(ref _path, value);
        }

        protected override IEnumerable<object> getAtomicValues()
            => new object[] { Name, Path };
        
        public override string ToString() => $"Name:'{Name}' | Path:'{Path}' | Id:'{Id}'";
    }

    public class DirectorySetupConfiguration : IEntityTypeConfiguration<DirectorySetup>
    {
        public static string TableName => "Directories";
        public void Configure(EntityTypeBuilder<DirectorySetup> builder)
        {
            builder.ToTable(TableName);
            builder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            builder.OwnsOne(x => x.Name,o=>
                {
                    o.WithOwner();
                    o.Property(x => x.Value)
                        .IsRequired()
                        .HasColumnName("Name")
                        .UsePropertyAccessMode(PropertyAccessMode.Property);
                });
            
            builder.OwnsOne(x => x.Path, o =>
                {
                    o.WithOwner();
                    o.Property(x => x.Value)
                        .IsRequired()
                        .HasColumnName("Path")
                        .UsePropertyAccessMode(PropertyAccessMode.Field);
                });


            builder.Ignore(x => x.Id);

            builder.Property(x => x.Guid)
                .HasColumnName("Id")
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasValueGenerator<GuidValueGenerator>();
            builder.HasKey(x => x.Guid);
        }
    }
}