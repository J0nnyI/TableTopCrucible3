using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.ChangeSets;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IScannedFileRepository
    : IRepository<ScannedFileDataId, ScannedFileDataModel, ScannedFileDataEntity, ScannedFileDataChangeSet>
    {
    }
    internal class ScannedFileRepository
        : RepositoryBase<ScannedFileDataId, ScannedFileDataModel, ScannedFileDataEntity, ScannedFileDataChangeSet>,
        IScannedFileRepository
    {
        public ScannedFileRepository(IDatabaseAccessor database)
            : base(database, database.Files)
        { }
    }
}
