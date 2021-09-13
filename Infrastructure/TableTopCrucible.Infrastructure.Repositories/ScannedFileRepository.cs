
using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories
{
    [Singleton(typeof(ScannedFileRepository))]
    public interface IScannedFileRepository :
        ISourceRepository<ScannedFileDataId, ScannedFileData, ScannedFileDataDto>
    {
    }
    internal class ScannedFileRepository :
        SourceRepositoryBase<ScannedFileDataId, ScannedFileData, ScannedFileDataDto>,
        IScannedFileRepository
    {
        public ScannedFileRepository(IDatabase database) : base(database)
        {
        }

    }
}
