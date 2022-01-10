using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IImageDataRepository:IRepository<ImageDataId, ImageData>
    {

    }
    class ImageDataDataRepository:RepositoryBase<ImageDataId, ImageData>,IImageDataRepository
    {
        public ImageDataDataRepository(IDatabaseAccessor database, DbSet<ImageData> data) : base(database, data)
        {
        }

        public override string TableName => ImageDataConfiguration.TableName;
    }
}
