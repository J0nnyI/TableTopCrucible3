using System;
using System.Linq;
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
                    vm => vm.SingleItemViewer,
                    v => v.SingleItemViewer.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.ItemActions,
                    v => v.Actions.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.ListHeader,
                    v => v.ListHeader.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.Filter,
                    v => v.Filter.ViewModel),
                this.OneWayBind(ViewModel,
                    vm=>vm.SelectionErrorText,
                    v=>v.SelectionErrorDisplay.Text),
            });
        }

    }
}