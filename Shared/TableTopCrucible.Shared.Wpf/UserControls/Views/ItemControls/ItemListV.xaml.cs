using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views.ItemControls
{
    /// <summary>
    ///     Interaction logic for ItemListV.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class ItemListV
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

        private void ListViewItem_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem { Content: ItemSelectionInfo itemInfo })
                ViewModel.OnItemClicked(itemInfo, e);
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                ViewModel.InitiateDrag(sender as DependencyObject);
        }

        private void DisableDelegation(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}