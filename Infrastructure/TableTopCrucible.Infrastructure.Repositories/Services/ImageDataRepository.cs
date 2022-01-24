using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
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
        FileData GetThumbnail(ItemId itemId);
        IObservable<FileData> WatchThumbnail(ItemId itemId);
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

        public IObservable<FileData> WatchThumbnail(ItemId itemId)
            => itemId is null
                ? Observable.Return<FileData>(null)
                : // itemId => all item images
                    WatchMany(itemId)
                    .ToCollection()
                    // item images => { image, isThumbnail }[] changes
                    .Select(images =>
                        images
                            .Select(img =>
                                img.WhenAnyValue(i => i.IsThumbnail)
                                    .Select(isThumbnail => new { img, isThumbnail }))
                            .CombineLatest()
                    ).Switch()
                    // { image, isThumbnail }[] changes => imageData
                    .Select(images =>
                        images
                            // order by to get the thumbnail files first
                            .OrderBy(imgInfo => !imgInfo.isThumbnail)
                            // { image, isThumbnail }[] => image[]
                            .Select(imgInfo => imgInfo.img)
                            // image[] => fileData changes[]
                            .Select(img=>img
                                .WhenAnyValue(i=>i.HashKey)
                                .Select(hashKey=>_fileRepository.WatchSingle(hashKey))
                                .Switch())
                            // fileData changes[] => fileData changes
                            .CombineLatest(files => // the first file which exists
                                files.FirstOrDefault(img=>img?.Path.Exists()==true)
                            )
                        )
                    .Switch();

        public IEnumerable<ImageData> GetMany(ItemId itemId)
            => itemId is null
                ? Enumerable.Empty<ImageData>()
                : Data.Items.Where(img => img.ItemId == itemId);

        public FileData GetThumbnail(ItemId itemId)
            => GetMany(itemId).OrderBy(x=>!x.IsThumbnail)
                .Select(img => _fileRepository.SingleByHashKey(img.HashKey))
                .FirstOrDefault(file => file is not null);
    }
}