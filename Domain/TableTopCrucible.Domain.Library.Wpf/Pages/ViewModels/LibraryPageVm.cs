﻿using System;
using System.Reactive.Disposables;
using DynamicData;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels;

[Singleton]
public interface ILibraryPage : INavigationPage
{
}

public class LibraryPageVm : ReactiveObject, IActivatableViewModel, ILibraryPage, IDisposable
{
    public LibraryPageVm(
        //left
        IItemList itemList,
        IFilteredListHeader listHeader,
        IItemListFilter filter,
        IDirectoryItemBrowser directoryItemBrowser,
        //right
        IItemActions itemActions,
        IItemViewer itemViewer,
        //services
        ILibraryService libraryService)
    {
        ItemList = itemList.DisposeWith(_disposables);
        ListHeader = listHeader;
        Filter = filter;
        DirectoryItemBrowser = directoryItemBrowser;
        ItemActions = itemActions;
        ItemViewer = itemViewer;

        itemList.SelectedItems.Connect().PopulateInto(libraryService.SelectedItems);

        this.WhenActivated(() => new[]
        {
            filter.FilterChanges.BindTo(this, vm => vm.ItemList.Filter),
        });
    }

    private readonly CompositeDisposable _disposables = new();
    public ViewModelActivator Activator { get; } = new();

    // list (left)
    public IItemList ItemList { get; }
    public IFilteredListHeader ListHeader { get; }
    public IItemListFilter Filter { get; }
    public IDirectoryItemBrowser DirectoryItemBrowser { get; }

    // viewer (right)
    public IItemViewer ItemViewer { get; }
    public IItemActions ItemActions { get; }

    #region navigation tab

    public PackIconKind? Icon => PackIconKind.Bookshelf;
    public Name Title => Name.From("Item Library");
    public NavigationPageLocation PageLocation => NavigationPageLocation.Upper;
    public SortingOrder Position => SortingOrder.From(1);

    public void Dispose()
        => _disposables.Dispose();

    #endregion
}