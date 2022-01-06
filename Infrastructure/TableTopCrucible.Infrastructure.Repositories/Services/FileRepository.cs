using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IFileRepository
        : IRepository<FileDataId, FileData>
    {
    }

    internal class FileRepository
        : RepositoryBase<FileDataId, FileData>,
            IFileRepository
    {
        public FileRepository(IDatabaseAccessor database)
            : base(database, database.Files)
        {

        }

        public override string TableName => FileDataConfiguration.TableName;
    }
}