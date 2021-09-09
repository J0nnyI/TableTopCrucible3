using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Dtos
{
    public class DirectorySetupDto : IEntityDto<DirectorySetupId, DirectorySetup>
    {
        public Name Name { get; }
        public DirectorySetupPath Path { get; }
        public SynchronizationTime LastSync { get; }
    }
}
