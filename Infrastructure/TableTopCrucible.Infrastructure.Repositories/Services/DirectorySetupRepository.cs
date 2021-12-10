using System;
using System.Collections.Generic;

using DynamicData;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IDirectorySetupRepository
        : IRepository<DirectorySetupId, DirectorySetupEntity>
    {
    }
    internal class DirectorySetupRepository
        : RepositoryBase<DirectorySetupId, DirectorySetupEntity>,
        IDirectorySetupRepository
    {
        public DirectorySetupRepository(IDatabaseAccessor database) : base(database.DirectorySetup)
        {
        }
    }
}
