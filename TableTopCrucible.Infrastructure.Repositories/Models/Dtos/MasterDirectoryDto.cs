using System;
using System.Collections.Generic;
using System.Text;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Infrastructure.Repositories.Entities;
using TableTopCrucible.Infrastructure.Repositories.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Dtos
{
    public class MasterDirectoryDto:IEntityDto<MasterDirectoryId, MasterDirectory>
    {
        public Name Name { get; }
        public MasterDirectoryPath Path { get; }
        public SynchronizationTime LastSync { get; }
    }
}
