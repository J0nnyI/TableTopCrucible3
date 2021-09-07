using ReactiveUI;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for NotificationListV.xaml
    /// </summary>
    public partial class NavigationListV : ReactiveUserControl<NavigationListVm>, IActivatableView
    {
        public NavigationListV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v=>v.ViewModel).BindTo(this, v=>v.DataContext),
            });
        }
    }
}
