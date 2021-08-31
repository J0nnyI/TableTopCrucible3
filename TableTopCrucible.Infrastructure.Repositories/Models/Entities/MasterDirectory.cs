using System;
using System.Collections.Generic;
using System.Text;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Infrastructure.Repositories.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Entities
{
    public class MasterDirectory:IEntity<MasterDirectoryId>
    {
        public MasterDirectoryId Id { get; }
    }
}
