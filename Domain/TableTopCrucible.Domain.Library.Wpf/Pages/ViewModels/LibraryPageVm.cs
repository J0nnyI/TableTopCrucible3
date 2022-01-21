using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Security.Cryptography;

using DynamicData;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels
{
    [Singleton]
    public interface ILibraryPage : INavigationPage
    {
    }

    public class LibraryPageVm : ReactiveObject, IActivatableViewModel, ILibraryPage, IDisposable
    {
        private readonly IFileRepository _fileRepository;
        private readonly IImageDataRepository _imageDataRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly INotificationService _notificationService;

        public LibraryPageVm(
            IItemList itemList,
            IFilteredListHeader listHeader,
            IItemListFilter filter,
            IItemModelViewer modelViewer,
            IItemActions actions,
            IItemDataViewer dataViewer,
            IItemViewerHeader viewerHeader,
            IItemFileList fileList,
            IGallery gallery,
            IFileRepository fileRepository,
            IImageDataRepository imageDataRepository,
            IItemRepository itemRepository,
            IDirectorySetupRepository directorySetupRepository,
            INotificationService notificationService)
        {
            _fileRepository = fileRepository;
            _imageDataRepository = imageDataRepository;
            _itemRepository = itemRepository;
            _directorySetupRepository = directorySetupRepository;
            _notificationService = notificationService;
            ItemList = itemList.DisposeWith(_disposables);
            ListHeader = listHeader;
            Filter = filter;
            ModelViewer = modelViewer;
            Actions = actions;
            DataViewer = dataViewer;
            ViewerHeader = viewerHeader;
            Gallery = gallery;
            FileList = fileList.DisposeWith(_disposables);

            var itemChanges = ItemList.SelectedItems
                .Connect()
                .ToCollection()
                .Select(x => x.FirstOrDefault())
                .Publish()
                .RefCount();
            this.WhenActivated(() => new[]
            {
                itemChanges
                    .BindTo(this, vm => vm.ModelViewer.Item),
                itemChanges
                    .BindTo(this, vm => vm.DataViewer.Item),
                itemChanges
                    .BindTo(this, vm => vm.ViewerHeader.Item),
                itemChanges
                    .BindTo(this, vm => vm.FileList.Item),
                itemChanges
                    .BindTo(this, vm => vm.Gallery.Item),
            });
        }

        private readonly CompositeDisposable _disposables = new();
        public IItemList ItemList { get; }
        public IFilteredListHeader ListHeader { get; }
        public IItemListFilter Filter { get; }
        public IItemModelViewer ModelViewer { get; }
        public IItemActions Actions { get; }
        public IItemDataViewer DataViewer { get; }
        public IItemViewerHeader ViewerHeader { get; }
        public IGallery Gallery { get; }
        public IItemFileList FileList { get; }
        public ViewModelActivator Activator { get; } = new();
        public PackIconKind? Icon => PackIconKind.Bookshelf;
        public Name Title => Name.From("Item Library");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Upper;
        public SortingOrder Position => SortingOrder.From(1);
        public void Dispose()
            => _disposables.Dispose();

        public void HandleFileDrop(FilePath[] filePaths)
        {
            try
            {
                if (_directorySetupRepository.Data.Items.None())
                {
                    _notificationService.AddNotification((Name)"Could not add Images",
                        (Description)"You have to configure your directories first.", NotificationType.Error);
                    return;
                }

                // item
                var item = ItemList.SelectedItems.Items.FirstOrDefault();
                if (item is null)
                {
                    _notificationService.AddNotification((Name)"Could not add Images",
                        (Description)"You have to select an Item first.", NotificationType.Error);
                    return;

                }

                // fileData
                var foundFiles = _fileRepository.ByFilepath(filePaths).ToList();

                using var hash = new SHA512Managed();
                var hashInfo =
                    filePaths
                        .Where(file => file.IsImage())
                        .Except(foundFiles.Select(file => file.Path))
                        .Select(filePath =>
                        {
                            var hashKey = FileHashKey.Create(filePath, hash);
                            var foundFileData = _fileRepository[hashKey].FirstOrDefault();
                            var foundImageData = _imageDataRepository[hashKey].Where(img => img.ItemId == item.Id);
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
                                var modelFile = itemFile is null?null: _fileRepository[itemFile.HashKey].FirstOrDefault();
                                relatedDirSetup ??=
                                    (itemFile is null
                                        ? null
                                        : _directorySetupRepository.ByFilepath(modelFile.Path).FirstOrDefault())
                                    ?? _directorySetupRepository.Data.Items.FirstOrDefault();


                                var newPath = relatedDirSetup.ThumbnailDirectory +
                                    (filePath.GetFilenameWithoutExtension()
                                      + BareFileName.TimeSuffix
                                      + filePath.GetExtension());

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
                        .Select(x => new FileData(x.filePath, x.hashKey, x.filePath.GetLastWriteTime()))
                        .ToArray();
                _fileRepository.AddRange(newFiles);

                // image 1
                var newImages =
                    hashInfo
                        .Where(x => !x.foundImageData.Any())
                        .Select(x => new ImageData(x.filePath.GetFilenameWithoutExtension().ToName(), x.hashKey) { ItemId = item.Id })
                        .ToArray();
                _imageDataRepository.AddRange(newImages);

            }
            catch (Exception e)
            {
                Debugger.Break();

                _notificationService.AddNotification((Name)"Could not add Images",
                    (Description)string.Join(Environment.NewLine,"The Operation failed due to an internal error.","error:",e.ToString()), NotificationType.Error);
                throw;
            }
        }
    }
}