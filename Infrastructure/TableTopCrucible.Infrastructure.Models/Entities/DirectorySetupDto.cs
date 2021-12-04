using System;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Dtos
{
    public class DirectorySetupDto 
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid Id { get; set; }

        public DirectorySetup ToEntity()
        {
            return new(
                (Name)Name,
                (DirectorySetupPath)Path,
                (DirectorySetupId)Id
            );
        }

        public void Initialize(DirectorySetup sourceEntity)
        {
            Name = sourceEntity.Name.Value;
            Path = sourceEntity.Path.Value;
            Id = sourceEntity.Id.Value;
        }
    }
}
