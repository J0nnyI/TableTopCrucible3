using System;

using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Dtos
{
    public class DirectorySetupDto : IEntityDto<DirectorySetupId, DirectorySetup>
    {
        public string Name { get; }
        public string Path { get; }
        public DateTime LastSync { get; }
        public Guid IdValue { get; set; }
    }
}
