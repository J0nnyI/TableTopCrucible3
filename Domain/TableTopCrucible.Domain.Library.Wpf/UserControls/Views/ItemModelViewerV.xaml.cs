using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views.ItemControls
{
    /// <summary>
    ///     Interaction logic for ItemModelViewerV.xaml
    /// </summary>
    public partial class ItemModelViewerV : ReactiveUserControl<ItemModelViewerVm>
    {
        public ItemModelViewerV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.Bind(ViewModel,
                    vm => vm.ModelViewer,
                    v => v.ModelViewer.ViewModel)
            });
        }
    }
}