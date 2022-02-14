using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Services;

[Singleton]
public interface IGalleryService
{
    public IEnumerable<ImageData> AddImagesToItem(Item item, params ImageFilePath[] files);
    void SetImageToThumbnail(ImageData image);
    void AddThumbnailToItem(Item item, ImageFilePath file);
}

internal class GalleryService : IGalleryService
{
    private readonly IDirectorySetupRepository _directorySetupRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IItemRepository _itemRepository;
    private readonly INotificationService _notificationService;

    public GalleryService(
        IFileRepository fileRepository,
        IImageRepository imageRepository,
        IItemRepository itemRepository,
        IDirectorySetupRepository directorySetupRepository,
        INotificationService notificationService)
    {
        _fileRepository = fileRepository;
        _imageRepository = imageRepository;
        _itemRepository = itemRepository;
        _directorySetupRepository = directorySetupRepository;
        _notificationService = notificationService;
    }

    /// <summary>
    ///     does not generate a thumbnail, use <see cref="IThumbnailGenerationService.Generate" /> instead
    /// </summary>
    /// <param name="item"></param>
    /// <param name="file"></param>
    public void AddThumbnailToItem(Item item, ImageFilePath file)
    {
        var image = AddImagesToItem(item, file).First();
        SetImageToThumbnail(image);
    }

    /// <summary>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="files"></param>
    /// <returns>a list of all related images, unsorted, regardless if touched or untouched</returns>
    public IEnumerable<ImageData> AddImagesToItem(Item item, params ImageFilePath[] files)
    {
        try
        {
            if (_directorySetupRepository.Data.Items.None())
            {
                _notificationService.AddNotification((Name)"Could not add Images",
                    (Description)"You have to configure your directories first.", NotificationType.Error);
                return Enumerable.Empty<ImageData>();
            }

            // item
            if (item is null)
            {
                _notificationService.AddNotification((Name)"Could not add Images",
                    (Description)"You have to select an Item first.", NotificationType.Error);
                return Enumerable.Empty<ImageData>();
            }

            // fileData
            var foundFiles = _fileRepository
                .ByFilepath(files.Select(img => img.ToFilePath()))
                .Select(file => file.Path.ToImagePath())
                .ToList();

            using var hash = new SHA512Managed();
            var hashInfo =
                files
                    .Except(foundFiles.Select(file => file))
                    .Select(filePath =>
                    {
                        var hashKey = FileHashKey.Create(filePath, hash);
                        var foundFileData = _fileRepository[hashKey].Where(file => file.Path.Exists())
                            .FirstOrDefault();
                        var foundImageData = _imageRepository[hashKey].Where(img => img.ItemId == item.Id);
                        /*** priority:
                         * 1.- dir setup of the thumbnail (null if it is not in any tracked directory)
                         * 2.- dir setup of the related model file (null if no model is linked to this item)
                         * 3.- first dir setup in the table
                         * 4.- no configured dir-setup is catched via guard at the beginning
                         */
                        var relatedDirSetup = _directorySetupRepository.ByFilepath(filePath).FirstOrDefault();


                        if (foundFileData is null)
                        {
                            var itemFile = _fileRepository[item.FileKey3d].FirstOrDefault();
                            var modelFile = itemFile is null
                                ? null
                                : _fileRepository[itemFile.HashKey].FirstOrDefault();
                            relatedDirSetup ??=
                                (itemFile is null
                                    ? null
                                    : _directorySetupRepository.ByFilepath(modelFile.Path).FirstOrDefault())
                                ?? _directorySetupRepository.Data.Items.FirstOrDefault()
                                ?? throw new NullReferenceException(nameof(relatedDirSetup));


                            var newPath =
                                ImageFilePath.From(
                                    relatedDirSetup.ThumbnailDirectory,
                                    filePath.GetFilenameWithoutExtension() + BareFileName.TimeSuffix,
                                    filePath.GetExtension());

                            relatedDirSetup.ThumbnailDirectory.Create();
                            filePath.Copy(newPath);
                            filePath = newPath;
                        }

                        return new
                        {
                            filePath,
                            hashKey,
                            foundImageData,
                            foundFileData,
                            relatedDirSetup
                        };
                    })
                    .ToArray();

            var newFiles =
                hashInfo
                    .Where(x => x.foundFileData is null)
                    .Select(x => new FileData(x.filePath.ToFilePath(), x.hashKey, x.filePath.GetLastWriteTime()))
                    .ToArray();
            _fileRepository.AddRange(newFiles);

            // image 1
            var newImages =
                hashInfo
                    .Where(x => !x.foundImageData.Any())
                    .Select(x => new ImageData(x.filePath.GetFilenameWithoutExtension().ToName(), x.hashKey)
                        { ItemId = item.Id })
                    .ToArray();
            _imageRepository.AddRange(newImages);
            return hashInfo.SelectMany(info => info.foundImageData)
                .Where(x => x is not null)
                .Concat(newImages);
        }
        catch (Exception e)
        {
            Debugger.Break();

            _notificationService.AddNotification((Name)"Could not add Images",
                (Description)string.Join(Environment.NewLine, "The Operation failed due to an internal error.",
                    "error:", e.ToString()), NotificationType.Error);
            throw;
        }
    }

    public void SetImageToThumbnail(ImageData image)
    {
        image.IsThumbnail = true;
        _imageRepository
            .GetMany(image.ItemId)
            .Where(img => img.IsThumbnail && img != image)
            .ToList()
            .ForEach(img => img.IsThumbnail = false);
    }
}