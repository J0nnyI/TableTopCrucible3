using DynamicData;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    [Singleton(typeof(SourceDirectoryService))]
    public interface ISourceDirectoryService
    {
    }
    internal class SourceDirectoryService : ISourceDirectoryService
    {
    }
}
