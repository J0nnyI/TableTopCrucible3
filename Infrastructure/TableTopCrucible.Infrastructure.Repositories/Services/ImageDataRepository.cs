using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IImageDataRepository:IRepository<ImageDataId, ImageData>
    {
        public IQueryable<ImageData> this[FileHashKey hashKey] { get; }

    }
        class ImageDataRepository:RepositoryBase<ImageDataId, ImageData>,IImageDataRepository
    {
        public ImageDataRepository(IDatabaseAccessor database) : base(database, database.Images)
        {
        }

        public IQueryable<ImageData> this[FileHashKey hashKey]
            => hashKey is null
                ? Enumerable.Empty<ImageData>().AsQueryable()
                : this.Data.Where(image => image.HashKeyRaw == hashKey.Value);

        public override string TableName => ImageDataConfiguration.TableName;
    }
}
