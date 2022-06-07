using DynamicData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IZipMappingRepository:IRepository<ZipMappingId, ZipMapping> { }
    internal class ZipMappingRepository : RepositoryBase<ZipMappingId, ZipMapping>, IZipMappingRepository
    {
        public ZipMappingRepository(
            IStorageController storageController) 
            : base(storageController, storageController.ZipMappings)
        {
        }
    }
}
