using System.Diagnostics.CodeAnalysis;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;
using TableTopCrucible.Infrastructure.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.ChangeSets
{

    public class DirectorySetupChangeSet:IDataChangeSet<DirectorySetupId, DirectorySetupModel, DirectorySetupEntity>
    {
        [NotNull] public DirectorySetupId Id { get; set; }
        [NotNull] public Name Name { get; set; }
        [NotNull] public DirectorySetupPath Path { get; set; }

        public DirectorySetupModel ToModel()
        {
            return new (
                this.Name,
                this.Path,
                this.Id
            );
        }

        public void UpdateEntity(DirectorySetupEntity entity)
        {
            entity.Name = this.Name.Value;
            entity.Path = this.Path.Value;
            entity.Id = this.Id.Value;
        }

        public DirectorySetupEntity ToEntity()
        {
            var entity = new DirectorySetupEntity();
            UpdateEntity(entity);
            return entity;
        }

        public DirectorySetupChangeSet()
        {

        }
        public DirectorySetupChangeSet(Name name, DirectorySetupPath path, DirectorySetupId id = null)
        {
            Id = id ?? DirectorySetupId.New();
            this.Name = name;
            this.Path = path;
        }
    }
}
