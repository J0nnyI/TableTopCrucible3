using System;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class DirectorySetupEntity:IDataEntity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid Id { get; set; }
    }
}
