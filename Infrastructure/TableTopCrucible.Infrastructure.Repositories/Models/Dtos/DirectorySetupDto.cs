using System;

using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Dtos
{
    public class DirectorySetupDto : IEntityDto<DirectorySetupId, DirectorySetup>
    {
        public string NameValue { get; set; }
        public string PathValue { get; set; }
        public DateTime LastSyncValue { get; set; }
        public Guid IdValue { get; set; }

    }
}
