﻿using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    ///     Interaction logic for AppHeaderV.xaml
    /// </summary>
    public partial class AppHeaderV : ReactiveUserControl<AppHeaderVm>, IActivatableView
    {
        public AppHeaderV()
        {
            InitializeComponent();
            var buttonBorderSelected =
                Application.Current.TryFindResource("PrimaryHueLightBrush") as Brush;
            var buttonBorder = Brushes.Transparent;
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.DataContext),

                ViewModel!.NotificationCountChanges
                    .Select(count =>
                        count <= 0 || count >= 100
                            ? string.Empty
                            : count.ToString())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.NotificationToggleBadge.Badge),

                ViewModel!.JobCountChanges
                    .Select(c =>
                        c > 0
                            ? c.ToString()
                            : string.Empty)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.JobBadge.Badge),

                ViewModel!.CurrentPageTitleChanges
                    .Select(title => title.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.CurrentPageTitle.Text),

                ViewModel!.GlobalJobProgressChanges
                    .Select(progress => (progress?.Value ?? 0) < 100
                        ? Visibility.Visible
                        : Visibility.Hidden)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.JobProgress.Visibility),

                ViewModel!.GlobalJobProgressChanges
                    .Select(progress => progress.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.JobProgress.Value),

                ViewModel!.IsNotificationSidebarSelectedChanged
                    .Select(selected => selected
                        ? PackIconKind.Bell
                        : PackIconKind.BellOutline)
                    .BindTo(this, v => v.NotificationToggleIcon.Kind),
                ViewModel!.IsJobQueueSelectedChanged
                    .Select(selected => selected
                        ? PackIconKind.Clock
                        : PackIconKind.ClockOutline)
                    .BindTo(this, v => v.JobToggleIcon.Kind),

                this.Bind(ViewModel,
                    vm => vm.IsNavigationBarExpanded,
                    v => v.IsNavigationBarExpanded.IsChecked,
                    RxApp.MainThreadScheduler),

                this.OneWayBind(ViewModel,
                    vm => vm.IsNavigationBarExpanded,
                    v => v.IsNavigationBarExpanded.ToolTip,
                    isExpanded => isExpanded
                        ? "Collapse Sidebar"
                        : "Expand Sidebar"),

                this.OneWayBind(ViewModel,
                    vm => vm.ShowJobSidebarCommand,
                    v => v.ShowJobSidebar.Command),

                this.OneWayBind(ViewModel,
                    vm => vm.SaveCommand,
                    v => v.Save.Command),

                this.OneWayBind(ViewModel,
                    vm => vm.ShowNotificationSidebar,
                    v => v.NotificationToggle.Command)
            });
        }
    }
}