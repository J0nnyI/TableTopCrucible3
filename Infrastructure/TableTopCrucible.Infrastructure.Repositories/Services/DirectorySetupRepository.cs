using System;
using System.Collections.Generic;

using DynamicData;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.ChangeSets;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;
using TableTopCrucible.Infrastructure.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IDirectorySetupRepository
        : IRepository<DirectorySetupId, DirectorySetupModel, DirectorySetupEntity, DirectorySetupChangeSet>
    {
    }
    internal class DirectorySetupRepository
        : RepositoryBase<DirectorySetupId, DirectorySetupModel, DirectorySetupEntity, DirectorySetupChangeSet>,
        IDirectorySetupRepository
    {
        public DirectorySetupRepository(IDatabaseAccessor database) : base(database, database.DirectorySetup)
        {
        }
    }
}
