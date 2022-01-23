using System.Windows.Controls;
using System.Windows.Input;
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
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Items,
                    v => v.Items.ItemsSource)
            });
        }

        private void ListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem lstItem &&
                lstItem.Content is ItemSelectionInfo itemInfo)
                ViewModel.OnItemClicked(itemInfo, e);
        }
    }
}