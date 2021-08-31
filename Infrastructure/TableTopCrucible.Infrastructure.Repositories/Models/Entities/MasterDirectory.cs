using System.Collections.Generic;
using DynamicData.Kernel;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Entities
{
    public class MasterDirectory:EntityBase<MasterDirectoryId>
    {
        public MasterDirectoryId Id { get; private set; }
        public Name Name { get; private set; }
        public MasterDirectoryPath Path { get; private set; }
        
    }
}
