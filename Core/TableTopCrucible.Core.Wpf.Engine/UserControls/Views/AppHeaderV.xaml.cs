using ReactiveUI;

using System.Linq;
using System.Reactive.Linq;

using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for AppHeaderV.xaml
    /// </summary>
    public partial class AppHeaderV : ReactiveUserControl<AppHeaderVm>, IActivatableView
    {
        public AppHeaderV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v=>v.ViewModel)
                    .BindTo(this, v=>v.DataContext),

                ViewModel!.NotificationCountChanges
                    .Select(count => count<=0?string.Empty:count.ToString())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.NotificationBadge.Badge),

                ViewModel!.JobCountChanges
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.JobBadge.Badge),

                this.OneWayBind(ViewModel,
                    vm=>vm.CurrentPageTitle,
                    v=>v.CurrentPageTitle.Text),

                this.Bind(ViewModel,
                    vm=>vm.IsNavigationbarExpanded,
                    v=>v.IsNavigationBarExpanded.IsChecked),

                this.OneWayBind(ViewModel,
                    vm => vm.IsNavigationbarExpanded,
                    v => v.IsNavigationBarExpanded.ToolTip,
                    (bool isExpanded) => isExpanded ? "Collapse Sidebar" : "Expand Sidebar"
                )
            });
        }
    }
}
