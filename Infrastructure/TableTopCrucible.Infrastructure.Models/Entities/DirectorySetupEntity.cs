using System;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;
using TableTopCrucible.Infrastructure.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class DirectorySetupEntity : IDataEntity<DirectorySetupId, DirectorySetupModel>
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid Id { get; set; }

        public DirectorySetupModel ToModel()
            => new((Name)Name, (DirectorySetupPath)Path, (DirectorySetupId)Id);
    }
}
