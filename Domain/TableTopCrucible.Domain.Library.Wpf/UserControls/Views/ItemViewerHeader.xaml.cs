using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views
{
    /// <summary>
    ///     Interaction logic for ModelHeaderV.xaml
    /// </summary>
    public partial class ItemViewerHeaderV : ReactiveUserControl<ItemViewerHeaderVm>
    {
        public ItemViewerHeaderV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                ViewModel!.TitleChanges
                    .BindTo(this, vm=>vm.Title.Text),
                ViewModel!.SelectionCountChanges
                    .Select(count=>count.ToString())
                    .BindTo(this, vm=>vm.ItemCount.Text),
                ViewModel!.ShowItemCount
                    .Select(visible=>visible?Visibility.Visible:Visibility.Collapsed)
                    .BindTo(this, vm=>vm.ItemCountBorder.Visibility),
                
                this.Bind(ViewModel,
                    vm=>vm.TabStrip,
                    v=>v.TabStrip.ViewModel)
            });
        }
    }
}