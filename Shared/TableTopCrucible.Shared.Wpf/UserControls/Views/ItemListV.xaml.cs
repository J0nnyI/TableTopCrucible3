using System;
using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    ///     Interaction logic for ItemListV.xaml
    /// </summary>
    public partial class ItemListV : ReactiveUserControl<ItemListVm>, IActivatableView
    {
        public ItemListV()
        {
            InitializeComponent();
            this.WhenActivated(() => new IDisposable[]
            {
                this.OneWayBind(ViewModel,
                    vm => vm.FileSyncCommand,
                    v => v.FileSync.Command),
                this.OneWayBind(ViewModel,
                    vm => vm.Files,
                    v => v.Files.ItemsSource),
                this.OneWayBind(ViewModel,
                    vm => vm.Items,
                    v => v.Items.ItemsSource)
            });
        }
    }
}