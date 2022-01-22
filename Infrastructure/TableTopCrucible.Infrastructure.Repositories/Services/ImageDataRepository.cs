using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IImageRepository : IRepository<ImageDataId, ImageData>
    {
        IEnumerable<ImageData> this[FileHashKey hashKey] { get; }
        IObservable<IChangeSet<ImageData, ImageDataId>> WatchMany(ItemId itemId);
        IEnumerable<ImageData> GetMany(ItemId itemId);
        FileData GetSingleFile(ItemId itemId);
    }

    internal class ImageRepository : RepositoryBase<ImageDataId, ImageData>, IImageRepository
    {
        private readonly IFileRepository _fileRepository;

        public ImageRepository(IStorageController storageController, IFileRepository fileRepository) : base(storageController,
            storageController.Images)
        {
            _fileRepository = fileRepository;
        }

        public IEnumerable<ImageData> this[FileHashKey hashKey]
            => hashKey is null
                ? Enumerable.Empty<ImageData>()
                : Data.Items.Where(image => image.HashKey == hashKey);


        public IObservable<IChangeSet<ImageData, ImageDataId>> WatchMany(ItemId itemId)
            => itemId is null
                ? Observable.Return(ChangeSet<ImageData, ImageDataId>.Empty)
                : Data.Connect().Filter(image => image.ItemId == itemId);

        public IEnumerable<ImageData> GetMany(ItemId itemId)
            => itemId is null
                ? Enumerable.Empty<ImageData>()
                : Data.Items.Where(img => img.ItemId == itemId);

        public FileData GetSingleFile(ItemId itemId)
            => GetMany(itemId)
                .Select(img => _fileRepository.SingleByHashKey(img.HashKey))
                .FirstOrDefault(file => file is not null);
    }
}