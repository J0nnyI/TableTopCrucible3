using System;

using DynamicData;

using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Infrastructure.Repositories
{
    [Singleton(typeof(MasterDirectoryRepository))]
    public interface IMasterDirectoryRepository :
        ISourceRepository<MasterDirectoryId, MasterDirectory, MasterDirectoryDto>
    {
    }
    internal class MasterDirectoryRepository :
        SourceRepositoryBase<MasterDirectoryId, MasterDirectory, MasterDirectoryDto>,
        IMasterDirectoryRepository
    {
        public MasterDirectoryRepository(IDatabase database) : base(database)
        {
        }
    }
}
