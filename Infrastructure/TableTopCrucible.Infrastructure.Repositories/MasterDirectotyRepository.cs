
using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

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
