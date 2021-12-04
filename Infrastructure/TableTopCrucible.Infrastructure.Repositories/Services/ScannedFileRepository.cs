
using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories
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
