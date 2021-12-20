using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IScannedFileRepository
        : IRepository<ScannedFileDataId, ScannedFileDataEntity>
    {
    }

    internal class ScannedFileRepository
        : RepositoryBase<ScannedFileDataId, ScannedFileDataEntity>,
            IScannedFileRepository
    {
        public ScannedFileRepository(IDatabaseAccessor database)
            : base(database, database.Files)
        {
        }
    }
}