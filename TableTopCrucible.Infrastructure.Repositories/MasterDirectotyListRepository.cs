using System;

using DynamicData;

using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Infrastructure.Repositories.Dtos;
using TableTopCrucible.Infrastructure.Repositories.Entities;
using TableTopCrucible.Infrastructure.Repositories.EntityIds;

using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Infrastructure.Repositories
{
    [Singleton(typeof(MasterDirectoryListRepository))]
    public interface IMasterDirectoryListRepository :
        ISourceRepository<MasterDirectoryId, MasterDirectory, MasterDirectoryDto>
    {
        IConnectableCache<MasterDirectory, MasterDirectoryId> Data { get; }
    }
    internal class MasterDirectoryListRepository :
        SourceRepositoryBase<MasterDirectoryId, MasterDirectory, MasterDirectoryDto>,
        IMasterDirectoryListRepository
    {
        public MasterDirectoryListRepository(IDatabase database) : base(database)
        {
        }
    }
}
