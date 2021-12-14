using System;
using System.ComponentModel.DataAnnotations.Schema;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class DirectorySetupEntity : IDataEntity<DirectorySetupId>
    {
        public Name Name { get; set; }
        public FilePath Path { get; set; }
        public DirectorySetupId Id { get; init; }
    }
}
