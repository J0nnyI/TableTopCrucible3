using DynamicData;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.DataTransfer.Services;
using TableTopCrucible.Data.Library.ValueTypes.IDs;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    [Singleton(typeof(SourceDirectoryService))]
    public interface ISourceDirectoryService
    {
        IObservableCache<SourceDirectory, SourceDirectoryId> Directories { get; }
        void AddOrUpdateDirectory(SourceDirectory directory);
        void RemoveSourceDirectory(SourceDirectoryId id);
    }
    internal class SourceDirectoryService : ISourceDirectoryService
    {
        private readonly SourceCache<SourceDirectory, SourceDirectoryId> _directories = new SourceCache<SourceDirectory, SourceDirectoryId>(dir => dir.Id);
        private readonly IDirectoryDataStorageService _directoryDataStorageService;

        public SourceDirectoryService(IDirectoryDataStorageService directoryDataStorageService)
        {
            _directoryDataStorageService = directoryDataStorageService;
        }
        public IObservableCache<SourceDirectory, SourceDirectoryId> Directories => _directories;
        public void AddOrUpdateDirectory(SourceDirectory directory)
        {
            this._directories.AddOrUpdate(directory);
        }
        public void RemoveSourceDirectory(SourceDirectoryId id)
        {
            this._directories.Remove(id);
        }
        public void Quicksave()
            => _directoryDataStorageService.Quicksave(this._directories.Items);
    }
}
