using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IDirectorySetupRepository
        : IRepository<DirectorySetupId, DirectorySetup>
    {
        public DirectorySetup this[DirectoryPath path] { get; }
    }

    internal class DirectorySetupRepository
        : RepositoryBase<DirectorySetupId, DirectorySetup>,
            IDirectorySetupRepository
    {
        public DirectorySetupRepository(IDatabaseAccessor database) : base(database, database.DirectorySetup)
        { }

        public DirectorySetup this[DirectoryPath path]
            => this.Data.SingleOrDefault(dir => dir.Path.Value == path.Value);
    }
}