using System.Collections.Generic;
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
        public IEnumerable<DirectorySetup> ByFilepath(FilePath path);
    }

    internal class DirectorySetupRepository
        : RepositoryBase<DirectorySetupId, DirectorySetup>,
            IDirectorySetupRepository
    {
        public DirectorySetupRepository(IStorageController storageController) : base(storageController, storageController.DirectorySetups)
        { }

        public DirectorySetup this[DirectoryPath path]
            => this.Data.Items.SingleOrDefault(dir => dir.Path.Value == path.Value);

        public IEnumerable<DirectorySetup> ByFilepath(FilePath path)
            => this.Data.Items.Where(dir => dir.Path.ContainsFilepath(path));
    }
}