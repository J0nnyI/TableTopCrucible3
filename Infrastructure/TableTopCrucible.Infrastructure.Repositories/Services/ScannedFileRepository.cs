using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IScannedFileRepository 
    {
    }
    internal class ScannedFileRepository :IScannedFileRepository
    {
        public ScannedFileRepository()
        {
        }

    }
}
