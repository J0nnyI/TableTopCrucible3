using ReactiveUI;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views;

/// <summary>
/// Interaction logic for ItemViewerV.xaml
/// </summary>
public partial class ItemViewerV
{
    public ItemViewerV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            this.Bind(ViewModel,
                vm => vm.Header,
                v => v.Header.ViewModel),
            this.Bind(ViewModel,
                vm => vm.TabContainer,
                v => v.TabContainer.ViewModel)
        });
    }
}