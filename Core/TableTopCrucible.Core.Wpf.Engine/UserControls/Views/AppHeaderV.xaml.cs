using System;
using ReactiveUI;

using System.Linq;
using System.Reactive.Linq;
using System.Windows;
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
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.DataContext),

                ViewModel!.NotificationCountChanges
                    .Select(count =>
                        count<=0 || count >= 100
                            ? string.Empty
                            : count.ToString())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.NotificationBadge.Badge),

                ViewModel!.JobCountChanges
                    .Select(c=>
                        c > 0
                        ? c.ToString()
                        : string.Empty)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.JobBadge.Badge),

                ViewModel!.CurrentPageTitleChanges
                    .Select(title=>title.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.CurrentPageTitle.Text),

                ViewModel!.GlobalJobProgressChanges
                    .Select(progress=> (progress?.Value ?? 0) < 100 ? Visibility.Visible : Visibility.Hidden)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.JobProgress.Visibility),

                this.Bind(ViewModel,
                    vm=>vm.IsNavigationbarExpanded,
                    v=>v.IsNavigationBarExpanded.IsChecked,
                    RxApp.MainThreadScheduler),

                ViewModel!.GlobalJobProgressChanges
                    .Select(progress=>progress.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.JobProgress.Value),

                this.OneWayBind(ViewModel,
                    vm => vm.IsNavigationbarExpanded,
                    v => v.IsNavigationBarExpanded.ToolTip,
                    isExpanded => isExpanded ? "Collapse Sidebar" : "Expand Sidebar"),
            });
        }
    }
}
