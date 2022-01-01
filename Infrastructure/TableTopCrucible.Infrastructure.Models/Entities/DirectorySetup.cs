using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
    }

    public class DirectorySetupConfiguration:IEntityTypeConfiguration<DirectorySetup>
    {
        public void Configure(EntityTypeBuilder<DirectorySetup> builder)
        {
            builder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            builder.OwnsOne(x => x.Name)
                .Property(x=>x.Value)
                .HasColumnName(nameof(DirectorySetup.Name));

            builder.OwnsOne(x => x.Path)
                .Property(x=>x.Value)
                .HasColumnName(nameof(DirectorySetup.Path));

            builder.Ignore(x => x.Id);

            builder.HasKey(x => x.Guid)
                .HasName(nameof(DirectorySetup.Id));
        }
    }
}