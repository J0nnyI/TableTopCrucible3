using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IScannedFileRepository
        : IRepository<FileDataId, FileData>
    {
    }

    internal class FileRepository
        : RepositoryBase<FileDataId, FileData>,
            IScannedFileRepository
    {
        public FileRepository(IDatabaseAccessor database)
            : base(database, database.Files)
        {

        }

        public override string TableName => FileDataConfiguration.TableName;
    }
}