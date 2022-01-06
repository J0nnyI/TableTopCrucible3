using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels
{
    [Singleton]
    public interface ILibraryPage : INavigationPage
    {
    }

    public class LibraryPageVm : ReactiveObject, IActivatableViewModel, ILibraryPage, IDisposable
    {
        public LibraryPageVm(
            IItemList itemList,
            IFilteredListHeader listHeader,
            IItemListFilter filter,
            IItemModelViewer modelViewer, 
            IItemActions actions, 
            IItemDataViewer dataViewer,
            IItemViewerHeader viewerHeader,
            IItemFileList fileList)
        {
            ItemList = itemList.DisposeWith(_disposables);
            ListHeader = listHeader;
            Filter = filter;
            ModelViewer = modelViewer;
            Actions = actions;
            DataViewer = dataViewer;
            ViewerHeader = viewerHeader;
            FileList = fileList.DisposeWith(_disposables);

            var itemChanges = ItemList.SelectedItems
                .Connect()
                .ToCollection()
                .Select(x => x.FirstOrDefault());
            itemChanges
                .BindTo(this, vm => vm.ModelViewer.Item)
                .DisposeWith(_disposables);
            itemChanges
                .BindTo(this, vm => vm.DataViewer.Item)
                .DisposeWith(_disposables);
            itemChanges
                .BindTo(this, vm => vm.ViewerHeader.Item)
                .DisposeWith(_disposables);
            itemChanges
                .BindTo(this, vm => vm.FileList.Item)
                .DisposeWith(_disposables);

        }

        private readonly CompositeDisposable _disposables = new();
        public IItemList ItemList { get; }
        public IFilteredListHeader ListHeader { get; }
        public IItemListFilter Filter { get; }
        public IItemModelViewer ModelViewer { get; }
        public IItemActions Actions { get; }
        public IItemDataViewer DataViewer { get; }
        public IItemViewerHeader ViewerHeader { get; }
        public IItemFileList FileList { get; }
        public ViewModelActivator Activator { get; } = new();
        public PackIconKind? Icon => PackIconKind.Bookshelf;
        public Name Title => Name.From("Item Library");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Upper;
        public SortingOrder Position => SortingOrder.From(1);
        public void Dispose()
            => _disposables.Dispose();
    }
}