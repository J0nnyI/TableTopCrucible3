﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DynamicData.Kernel;

using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Entities
{
    public class MasterDirectory : EntityBase<MasterDirectoryId>
    {
        [NotNull] public Name Name { get; init; }
        [NotNull] public MasterDirectoryPath Path { get; init; }
        
    }
}
