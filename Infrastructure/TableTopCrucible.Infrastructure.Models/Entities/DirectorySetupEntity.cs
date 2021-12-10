using System;
using System.ComponentModel.DataAnnotations.Schema;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class DirectorySetupEntity : IDataEntity<DirectorySetupId>
    {
        public string Name { get; set; }
        public string Path { get; set; }
        [DatabaseGenerated()]
        public Guid Id { get; init; }
    }
}
