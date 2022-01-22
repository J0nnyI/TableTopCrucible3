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
using TableTopCrucible.Domain.Library.Services;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Services;
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
        private readonly IGalleryService _galleryService;
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
            IGalleryService galleryService,
            INotificationService notificationService)
        {
            _fileRepository = fileRepository;
            _imageDataRepository = imageDataRepository;
            _itemRepository = itemRepository;
            _directorySetupRepository = directorySetupRepository;
            _galleryService = galleryService;
            _notificationService = notificationService;
            ItemList = itemList.DisposeWith(_disposables);
            ListHeader = listHeader;
            Filter = filter;
            ModelViewer = modelViewer;
            Actions = actions;
            Actions.GenerateThumbnailsCommand = ModelViewer.GenerateThumbnailCommand;
            DataViewer = dataViewer;
            ViewerHeader = viewerHeader;
            Gallery = gallery;
            FileList = fileList.DisposeWith(_disposables);

            var itemChanges = ItemList.SelectedItems
                .Connect()
                .ToCollection()
                .Select(x => x.FirstOrDefault())
                .Buffer(TimeSpan.FromMilliseconds(500))
                .Where(buffer=>buffer.Any())
                .Select(buffer=>buffer.Last())
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
            var images = 
                filePaths
                    .Where(file => file.IsImage())
                    .Select(img => img.ToImagePath())
                    .ToArray();
            var item = ItemList.SelectedItems.Items.FirstOrDefault();
            _galleryService.AddImagesToItem(item,images);
        }
    }
}