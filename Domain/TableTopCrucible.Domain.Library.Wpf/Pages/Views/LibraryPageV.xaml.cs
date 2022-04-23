using System;

using ReactiveUI;

using TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.Views;

/// <summary>
///     Interaction logic for LibraryPageV.xaml
/// </summary>
public partial class LibraryPageV : ReactiveUserControl<LibraryPageVm>
{
    public LibraryPageV()
    {
        InitializeComponent();
        this.WhenActivated(() =>
        {
            ItemList.ViewModel = ViewModel!.ItemList;
            Actions.ViewModel = ViewModel.ItemActions;
            ListHeader.ViewModel = ViewModel.ListHeader;
            DetailPage.ViewModel = ViewModel.ItemViewer;
            DirectoryList.ViewModel = ViewModel.DirectoryItemBrowser;
            return new IDisposable[]
            {
                this.WhenAnyValue(
                    v => v.ViewModel!.Filter,
                    v => v.DrawerHost.IsLeftDrawerOpen,
                    (content, isOpen) => isOpen ? content : null
                ).BindTo(this, v => v.Filter.ViewModel),

                    //this.Bind(ViewModel,
                    //    vm => vm.Filter,
                    //    v => v.Filter.ViewModel),
                    //this.WhenAnyValue(
                    //    vm=>vm.ViewModel.NoSelectionPlaceholder,
                    //    vm=>vm.ViewModel.SingleItemViewer,
                    //    vm=>vm.ViewModel.MultiItemViewer,
                    //    vm=>vm.ViewModel.Selection,
                    //    (noSelection, singleSelection, multiSelection,itemSelection)
                    //        => itemSelection.Select(
                    //            items=>
                    //                items.Count switch
                    //                {
                    //                    0 => noSelection as object,
                    //                    1 => singleSelection as object,
                    //                    _ => multiSelection as object
                    //                }
                    //            )
                    //        )
                    //    .Switch()
                    //    .ObserveOn(RxApp.MainThreadScheduler)
                    //    .BindTo(this,
                    //        v=>v.DetailPage.ViewModel)
            };
        });
    }
}