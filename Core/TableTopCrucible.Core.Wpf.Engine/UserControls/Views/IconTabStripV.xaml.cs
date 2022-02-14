using ReactiveUI;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views;

/// <summary>
/// Interaction logic for IconTabStripV.xaml
/// </summary>
public partial class IconTabStripV
{
    public IconTabStripV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            this.OneWayBind(ViewModel,
                vm => vm.Tabs,
                v => v.TabItems.ItemsSource)
        });
    }
}