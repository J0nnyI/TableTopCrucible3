using ReactiveUI;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views;

/// <summary>
/// Interaction logic for TabContainerV.xaml
/// </summary>
public partial class TabContainerV
{
    public TabContainerV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            this.OneWayBind(ViewModel,
                vm => vm.Tabs,
                v => v.TabControl.ItemsSource)
        });
    }
}