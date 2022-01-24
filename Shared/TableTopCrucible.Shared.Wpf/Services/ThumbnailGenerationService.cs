using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using HelixToolkit.Wpf;

using Microsoft.AspNetCore.Server.IIS.Core;

using Splat;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
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
        ImageFilePath Generate(Item item, HelixViewport3D viewport, CameraView view = null);
        ImageFilePath GenerateWithAutoPosition(Item item, HelixViewport3D viewport);

        public ImageFilePath Generate(ModelFilePath modelFile, Name sourceName, HelixViewport3D viewport = null);
    }
    internal class ThumbnailGenerationService : IWpfThumbnailGenerationService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly IGalleryService _galleryService;
        private readonly IProgressTrackingService _trackingService;

        public ThumbnailGenerationService(
            IItemRepository itemRepository,
            IFileRepository fileRepository,
            IImageRepository imageRepository,
            IDirectorySetupRepository directorySetupRepository,
            IGalleryService galleryService,
            IProgressTrackingService trackingService)
        {
            _itemRepository = itemRepository;
            _fileRepository = fileRepository;
            _imageRepository = imageRepository;
            _directorySetupRepository = directorySetupRepository;
            _galleryService = galleryService;
            _trackingService = trackingService;
        }

        public ImageFilePath Generate(Item item, CameraView view)
            => Generate(item, null, view);

        public ImageFilePath GenerateWithAutoPosition(Item item)
            => _generate(item, true, null, null);

        public ITrackingViewer GenerateManyAsync(IEnumerable<Item> items, ThreadCount parallelThreads = null,
            bool skipItemsWithThumbnails = false)
        {
            var tracker = _trackingService.CreateSourceTracker((Name)"Generate Thumbnails");
            var target = (TargetProgress)items.Select(item => item.FileKey3d.GetFileSizeComponent().Value).Sum();
            tracker.SetTarget(target);

            var source = items
                .Select(item => _fileRepository.SingleByHashKey(item.FileKey3d))
                .Select(Observable.Return)
                .Concat();

            GenerateManyAsync(source, tracker, parallelThreads);
            return tracker;
        }

        /// <summary>
        /// takes a stream of <see cref="FileData"/> and creates and links thumbnail images.
        /// </summary>
        /// <param name="source">pushes the files which require thumbnails<br/>the file is expected to have only one item.</param>
        /// <param name="tracker">must have a set target<br/>is incremented by the model fileSize</param>
        /// <param name="parallelThreads">number of parallel generating windows</param>
        /// <param name="skipItemsWithThumbnails"></param>
        public void GenerateManyAsync(
            IObservable<FileData> source,
            ISourceTracker tracker,
            ThreadCount parallelThreads = null,
            bool skipItemsWithThumbnails = false)
        {
            parallelThreads ??= (ThreadCount)SettingsHelper.SimultaneousThumbnailWindows;

            var thumbnailThrottle = new Subject<Unit>();
            source ??= new ReplaySubject<FileData>();

            var failedThumbnailGenerations = new List<FileData>();

            source.Zip(
                    thumbnailThrottle.StartWith(Enumerable
                        .Range(1, parallelThreads.Value)
                        .Select(_ => Unit.Default)),
                    (item, _) => item)
                .Subscribe(modelFile =>
                {
                    var modelSize = modelFile.HashKey.GetFileSizeComponent();
                    var item = _itemRepository.ByModelHash(modelFile.HashKey).First();
                    var img = _imageRepository.GetThumbnail(item.Id);
                    if (img is not null && skipItemsWithThumbnails) // item has a linked thumbnail
                    {
                        tracker.Increment((ProgressIncrement)modelSize.Value);
                        thumbnailThrottle.OnNext();
                        return;
                    }
                    var thread = new Thread(() =>
                    {
                        try
                        {
                            GenerateWithAutoPosition(item);
                        }
                        catch (COMException e) when (e.ErrorCode == -2003304445) // "MILERR_WIN32ERROR (0x88980003)"
                        {
                            try
                            {
                                GC.Collect();
                                GenerateWithAutoPosition(item);
                            }
                            catch (Exception exception)
                            {
                                failedThumbnailGenerations.Add(modelFile);
                                throw;
                            }
                        }
                        catch (Exception e)
                        {
                            failedThumbnailGenerations.Add(modelFile);
                            Debugger.Break();
                        }
                        finally
                        {
                            tracker.Increment((ProgressIncrement)modelSize.Value);
                            thumbnailThrottle.OnNext();
                        }
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                });

        }

        public ImageFilePath Generate(Item item, HelixViewport3D viewport, CameraView view = null)
            => _generate(item, false, view, viewport);

        public ImageFilePath GenerateWithAutoPosition(Item item, HelixViewport3D viewport)
            => _generate(item, true, null, viewport);

        public ImageFilePath Generate(ModelFilePath modelFile, Name sourceName, HelixViewport3D viewport = null)
            => _generate(modelFile, sourceName, viewport is null, null, viewport);

        private ImageFilePath _generate(Item item, bool autoPositionCamera, CameraView view, HelixViewport3D viewport)
        {
            var fileData = _fileRepository.SingleByHashKey(item.FileKey3d);
            var imgLocation = _generate(fileData.Path.ToModelPath(), item.Name, autoPositionCamera, view, viewport);
            _galleryService.AddThumbnailToItem(item, imgLocation);
            return imgLocation;
        }
        private ImageFilePath _generate(ModelFilePath modelFile, Name itemName, bool autoPositionCamera, CameraView view, HelixViewport3D viewport)
        {
            if (!modelFile.Exists())
                throw new InvalidOperationException($"no file found for item {itemName}");

            var dirSetup = _directorySetupRepository.SingleByFilepath(modelFile);

            var imgLocation = ImageFilePath.From(dirSetup.ThumbnailDirectory,
                itemName.ToFileName() + BareFileName.TimeSuffix, FileExtension.UncompressedImage);

            var visual = modelFile.LoadVisual(true);

            _generate(viewport, view, viewport is null, visual, imgLocation, autoPositionCamera);

            return imgLocation;
        }

        private void _generate(HelixViewport3D viewport, CameraView view, bool addDefaultLights, ModelVisual3D content, ImageFilePath imgPath, bool autoPositionCamera)
        {
            var usedViewport = viewport;
            Window window = null;
            try
            {
                if (viewport is null)
                {
                    usedViewport = new HelixViewport3D()
                    {
                        Width = SettingsHelper.ThumbnailSize.Width,
                        Height = SettingsHelper.ThumbnailSize.Height,
                        ShowViewCube = false
                    };
                    window = new()
                    {
                        Content = usedViewport,
                        Width = SettingsHelper.ThumbnailSize.Width * 2,
                        Height = SettingsHelper.ThumbnailSize.Height * 2,
                        Visibility = Visibility.Hidden,
                        ShowInTaskbar = false,
                        ShowActivated = false,
                        WindowStyle = WindowStyle.None,
                        Top = SettingsHelper.ThumbnailHeight * -3,
                        Left = SettingsHelper.ThumbnailWidth * -3,
                        Title = "Table Top Crucible Thumbnail Generator"
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

                Generate(usedViewport, imgPath);

            }
            finally
            {
                window?.Close();
            }
        }

        public void Generate(HelixViewport3D viewport, ImageFilePath imgPath)
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
