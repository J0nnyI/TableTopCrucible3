using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<DirectorySetup> ByFilepath<TFilePath>(FilePath<TFilePath> file)
            where TFilePath : FilePath<TFilePath>, new();

        public DirectorySetup SingleByFilepath<TFilePath>(FilePath<TFilePath> file)
            where TFilePath : FilePath<TFilePath>, new();
    }

    internal class DirectorySetupRepository
        : RepositoryBase<DirectorySetupId, DirectorySetup>,
            IDirectorySetupRepository
    {
        public DirectorySetupRepository(IStorageController storageController) : base(storageController,
            storageController.DirectorySetups)
        {
        }

        public DirectorySetup this[DirectoryPath path]
            => Data.Items.SingleOrDefault(dir => dir.Path.Value == path.Value);

        public IEnumerable<DirectorySetup> ByFilepath<TFilePath>(FilePath<TFilePath> file)
            where TFilePath : FilePath<TFilePath>, new()
            => Data.Items.Where(dir => dir.Path.ContainsFilepath(file));

        public DirectorySetup SingleByFilepath<TFilePath>(FilePath<TFilePath> file)
            where TFilePath : FilePath<TFilePath>, new()
            => Data.Items.FirstOrDefault(dir => dir.Path.ContainsFilepath(file));
    }
}