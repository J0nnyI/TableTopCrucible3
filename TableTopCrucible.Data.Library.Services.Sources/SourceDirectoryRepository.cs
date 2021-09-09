using System.Reflection.PortableExecutable;
using DynamicData;
using TableTopCrucible.Core.DataAccess;
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.Models.DataSource;
using TableTopCrucible.Data.Library.Models.DataTransfer;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    [Singleton(typeof(SourceDirectoryRepository))]
    public interface ISourceDirectoryService
    {
        IObservableCache<SourceDirectory, SourceDirectoryId> Directories { get; }
    }
    internal class SourceDirectoryRepository : ISourceDirectoryService
    {
        private readonly ITable<SourceDirectoryId, SourceDirectory, SourceDirectoryDto> table;

        public SourceDirectoryRepository( IDatabase database)
        {
            this.table = database.GetTable<SourceDirectoryId, SourceDirectory, SourceDirectoryDto>();
        }

        public IObservableCache<SourceDirectory, SourceDirectoryId> Directories => table.Data;

        AddOrUpdate
    }
}
