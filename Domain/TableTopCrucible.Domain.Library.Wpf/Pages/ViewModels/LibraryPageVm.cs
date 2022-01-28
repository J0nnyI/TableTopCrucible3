using System;
using System.Collections.Generic;
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
using TableTopCrucible.Infrastructure.Models.Entities;
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

        public LibraryPageVm(
            IItemList itemList,
            IFilteredListHeader listHeader,
            IItemListFilter filter,
            IGalleryService galleryService,
            ISingleItemViewer singleItemViewer,
            IItemActions itemActions,
            ILoadingScreen noSelectionPlaceholder,
            IMultiItemViewer multiItemViewer)
        {
            ItemList = itemList.DisposeWith(_disposables);
            ListHeader = listHeader;
            Filter = filter;
            SingleItemViewer = singleItemViewer;
            ItemActions = itemActions;
            NoSelectionPlaceholder = noSelectionPlaceholder;
            NoSelectionPlaceholder.Text = (Message)"No Item Selected";
            MultiItemViewer = multiItemViewer;

            ItemActions.GenerateThumbnailsByViewportCommand = singleItemViewer.GenerateThumbnailCommand;

            Selection = ItemList.SelectedItems
                .Connect()
                .StartWithEmpty()
                .ToCollection()
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Replay(1)
                .RefCount();

            this.WhenActivated(() => new[]
            {
                filter.FilterChanges.BindTo(this, vm=>vm.ItemList.Filter),

                Selection.Select(x => x.OnlyOrDefault())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Publish()
                    .RefCount()
                    .BindTo(this, vm=>vm.SingleItemViewer.Item),
            });
        }

        public IObservable<IReadOnlyCollection<Item>> Selection { get; }

        private readonly CompositeDisposable _disposables = new();
        public IItemList ItemList { get; }
        public IFilteredListHeader ListHeader { get; }
        public IItemListFilter Filter { get; }
        public ISingleItemViewer SingleItemViewer { get; }
        public IItemActions ItemActions { get; }
        public ILoadingScreen NoSelectionPlaceholder { get; }
        public IMultiItemViewer MultiItemViewer { get; }
        public ViewModelActivator Activator { get; } = new();

        public PackIconKind? Icon => PackIconKind.Bookshelf;
        public Name Title => Name.From("Item Library");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Upper;
        public SortingOrder Position => SortingOrder.From(1);
        public void Dispose()
            => _disposables.Dispose();
    }
}