using System;

using ReactiveUI;

using TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.Views
{
    /// <summary>
    /// Interaction logic for LibraryPageV.xaml
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
                    v => v.ItemList.ViewModel)
            });
        }
    }
}
