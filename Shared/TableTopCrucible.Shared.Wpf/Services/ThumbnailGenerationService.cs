using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using HelixToolkit.Wpf;

using Splat;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Helper;
using TableTopCrucible.Shared.Services;
using TableTopCrucible.Shared.ValueTypes;

namespace TableTopCrucible.Domain.Library.Services
{
    [Singleton]
    public interface IWpfThumbnailGenerationService : IThumbnailGenerationService
    {
        /// <summary>
        /// generates a thumbnail for the item and adds it to its gallery
        /// </summary>
        /// <param name="item"></param>
        /// <param name="view"></param>
        /// <param name="viewport">optional</param>
        /// <returns></returns>
        ImageFilePath GenerateThumbnail(Item item, HelixViewport3D viewport, CameraView view = null);
        ImageFilePath GenerateAutoPositionThumbnail(Item item, HelixViewport3D viewport);

        public ImageFilePath GenerateThumbnail(ModelFilePath modelFile, Name sourceName,HelixViewport3D viewport = null);
    }
    internal class ThumbnailGenerationService : IWpfThumbnailGenerationService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly IGalleryService _galleryService;

        public ThumbnailGenerationService(IItemRepository itemRepository, IFileRepository fileRepository, IDirectorySetupRepository directorySetupRepository, IGalleryService galleryService)
        {
            _itemRepository = itemRepository;
            _fileRepository = fileRepository;
            _directorySetupRepository = directorySetupRepository;
            _galleryService = galleryService;
        }

        public ImageFilePath GenerateThumbnail(Item item, CameraView view)
            => GenerateThumbnail(item, null, view);

        public ImageFilePath GenerateAutoPositionThumbnail(Item item)
            => _generateThumbnail(item, true, null, null);

        public ImageFilePath GenerateThumbnail(Item item, HelixViewport3D viewport, CameraView view = null)
            => _generateThumbnail(item, false, view, viewport);

        public ImageFilePath GenerateAutoPositionThumbnail(Item item, HelixViewport3D viewport)
            => _generateThumbnail(item, true, null, viewport);

        public ImageFilePath GenerateThumbnail(ModelFilePath modelFile, Name sourceName, HelixViewport3D viewport = null)
            => _generateThumbnail(modelFile, sourceName, viewport is null, null, viewport);

        private ImageFilePath _generateThumbnail(Item item, bool autoPositionCamera, CameraView view, HelixViewport3D viewport)
        {
            var fileData = _fileRepository.SingleByHashKey(item.FileKey3d);
            var imgLocation = _generateThumbnail(fileData.Path.ToModelPath(), item.Name, autoPositionCamera, view, viewport);
            _galleryService.AddThumbnailToItem(item, imgLocation);
            return imgLocation;
        }
        private ImageFilePath _generateThumbnail(ModelFilePath modelFile, Name itemName, bool autoPositionCamera, CameraView view, HelixViewport3D viewport)
        {
            if (!modelFile.Exists())
                throw new InvalidOperationException($"no file found for item {itemName}");

            var dirSetup = _directorySetupRepository.SingleByFilepath(modelFile);

            var imgLocation = ImageFilePath.From(dirSetup.ThumbnailDirectory,
                itemName.ToFileName() + BareFileName.TimeSuffix, FileExtension.UncompressedImage);

            var visual = modelFile.LoadVisual();

            _generateThumbnail(viewport, view, viewport is null, visual, imgLocation, autoPositionCamera);

            return imgLocation;
        }

        private void _generateThumbnail(HelixViewport3D viewport, CameraView view, bool addDefaultLights, ModelVisual3D content, ImageFilePath imgPath, bool autoPositionCamera)
        {
            var usedViewport =viewport;
            Window window =null;
            try
            {
                if (viewport is null)
                {
                    usedViewport = new HelixViewport3D()
                    {
                        Width = SettingsHelper.ThumbnailSize.Width,
                        Height = SettingsHelper.ThumbnailSize.Height
                    };
                    window = new()
                    {
                        Content = usedViewport,
                        Width = SettingsHelper.ThumbnailSize.Width * 2,
                        Height = SettingsHelper.ThumbnailSize.Height * 2,
                        Visibility = Visibility.Hidden,
                        ShowInTaskbar = false,
                    };
                    window.Show();
                }

                if (view is not null && autoPositionCamera)
                    throw new InvalidOperationException(
                        $"{nameof(view)} and {nameof(autoPositionCamera)} are mutually exclusive");

                if (view is not null)
                    usedViewport.ApplyCameraView(view);


                if (addDefaultLights)
                    usedViewport.Children.Add(new DefaultLights());

                if (content is not null)
                    usedViewport.Children.Add(content);

                if (autoPositionCamera)
                    usedViewport.Camera.ZoomExtents(usedViewport.Viewport, content?.FindBounds(content.Transform) ?? usedViewport.Children.FindBounds());

                GenerateThumbnail(usedViewport, imgPath);

            }
            finally
            {
                window?.Close();
            }
        }

        public void GenerateThumbnail(HelixViewport3D viewport, ImageFilePath imgPath)
        {
            var source = viewport.CreateBitmap();

            imgPath.GetDirectoryPath().Create();

            using var fileStream = imgPath.OpenWrite();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(fileStream);
        }

    }
}
