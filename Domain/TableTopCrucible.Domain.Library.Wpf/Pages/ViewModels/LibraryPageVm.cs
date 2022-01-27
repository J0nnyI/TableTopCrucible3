using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Services;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels
{
    [Singleton]
    public interface ILibraryPage : INavigationPage
    {
    }

    public class LibraryPageVm : ReactiveObject, IActivatableViewModel, ILibraryPage, IDisposable
    {
        private readonly IGalleryService _galleryService;

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
            IGalleryService galleryService)
        {
            _galleryService = galleryService;
            ItemList = itemList.DisposeWith(_disposables);
            ListHeader = listHeader;
            Filter = filter;
            ModelViewer = modelViewer;
            Actions = actions;
            Actions.GenerateThumbnailsByViewportCommand = ModelViewer.GenerateThumbnailCommand;
            Actions.SelectedItems = ItemList.SelectedItems;
            DataViewer = dataViewer;
            ViewerHeader = viewerHeader;
            Gallery = gallery;
            FileList = fileList.DisposeWith(_disposables);

            var selection = ItemList.SelectedItems
                .Connect()
                .StartWithEmpty()
                .ToCollection()
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Publish()
                .RefCount();

            var itemChanges = 
                selection.Select(x => x.OnlyOrDefault())
                .ObserveOn(RxApp.MainThreadScheduler)
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
                itemChanges
                    .BindTo(this, vm => vm.Actions.Item),
                filter.FilterChanges.BindTo(this, vm=>vm.ItemList.Filter),
                this._selectionErrorText = selection
                    .Select(items=> 
                        items.Count switch
                        {
                            0 => (Message)"No Item Selected",
                            1 => null,
                            _ => (Message)"Multi selection is not supported yet"
                        })
                    .ToProperty(this,vm=>vm.SelectionErrorText,false,RxApp.MainThreadScheduler)
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
        private ObservableAsPropertyHelper<Message> _selectionErrorText;
        public Message SelectionErrorText => _selectionErrorText.Value;
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