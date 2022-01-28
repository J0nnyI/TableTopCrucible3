using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

using ReactiveUI;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.Views
{
    /// <summary>
    ///     Interaction logic for LibraryPageV.xaml
    /// </summary>
    public partial class LibraryPageV : ReactiveUserControl<LibraryPageVm>
    {
        public LibraryPageV()
        {
            InitializeComponent();
            this.WhenActivated(() => new IDisposable[]
            {
                this.Bind(ViewModel,
                    vm => vm.ItemList,
                    v => v.ItemList.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.ItemActions,
                    v => v.Actions.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.ListHeader,
                    v => v.ListHeader.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.Filter,
                    v => v.Filter.ViewModel),
                this.WhenAnyValue(
                    vm=>vm.ViewModel.NoSelectionPlaceholder,
                    vm=>vm.ViewModel.SingleItemViewer,
                    vm=>vm.ViewModel.MultiItemViewer,
                    vm=>vm.ViewModel.Selection,
                    (noSelection, singleSelection, multiSelection,itemSelection)
                        => itemSelection.Select(
                            items=>
                                items.Count switch
                                {
                                    0 => noSelection as object,
                                    1 => singleSelection as object,
                                    _ => multiSelection as object
                                }
                            )
                        )
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this,
                        v=>v.DetailPage.ViewModel)

            });
        }

    }
}