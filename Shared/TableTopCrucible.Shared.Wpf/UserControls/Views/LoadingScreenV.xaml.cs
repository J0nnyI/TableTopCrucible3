using ReactiveUI;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views;

/// <summary>
/// Interaction logic for LoadingScreenV.xaml
/// </summary>
public partial class LoadingScreenV
{
    public LoadingScreenV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            this.OneWayBind(ViewModel,
                vm => vm.Text,
                v => v.LoadingScreen.Text)
        });
    }
}