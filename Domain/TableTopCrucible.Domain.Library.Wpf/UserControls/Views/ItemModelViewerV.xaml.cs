using ReactiveUI;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views;

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