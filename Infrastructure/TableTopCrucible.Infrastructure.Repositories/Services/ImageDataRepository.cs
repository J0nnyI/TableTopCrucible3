using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.EntityFrameworkCore;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IImageDataRepository : IRepository<ImageDataId, ImageData>
    {
        public IEnumerable<ImageData> this[FileHashKey hashKey] { get; }
        IObservable<IChangeSet<ImageData, ImageDataId>> ByItemId(ItemId itemId);

    }
    internal class ImageDataRepository : RepositoryBase<ImageDataId, ImageData>, IImageDataRepository
    {
        public ImageDataRepository(IStorageController storageController) : base(storageController, storageController.Images)
        {
        }

        public IEnumerable<ImageData> this[FileHashKey hashKey]
            => hashKey is null
                ? Enumerable.Empty<ImageData>()
                : this.Data.Items.Where(image => image.HashKey == hashKey);


        public IObservable<IChangeSet<ImageData, ImageDataId>> ByItemId(ItemId itemId)
            => itemId is null
                ? Observable.Return(ChangeSet<ImageData, ImageDataId>.Empty)
                : this.Data.Connect().Filter(image => image.ItemId == itemId);
    }
}
