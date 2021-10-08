using System;
using System.Reactive.Linq;
using ReactiveUI;

using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.Views
{
    /// <summary>
    /// Interaction logic for MainPageV.xaml
    /// </summary>
    public partial class MainPageV : ReactiveUserControl<MainPageVm>
    {
        public MainPageV()
        {
            InitializeComponent();



            this.WhenActivated(() => new IDisposable[]
            {
                ViewModel!.ActiveWorkAreaChanges
                    .BindTo(this,
                        v=>v.MainContainer.ViewModel),
                ViewModel!.ActiveSidebarChanges
                    .BindTo(this,
                        v=>v.SidebarContainer.ViewModel),
                ViewModel.ActiveSidebarChanges
                    .Select(sidebar=>sidebar != null)
                    .DistinctUntilChanged()
                    .BindTo(this, v=> v.DrawerHost.IsRightDrawerOpen),
                ViewModel!.ActiveSidebarChanges
                    .Select(sidebar=>sidebar?.Title)
                    .BindTo(this,
                        v=>v.SidebarTitle.Text),
                ViewModel!.ActiveSidebarChanges
                    .Select(sidebar=>sidebar?.Width?.Value ?? SidebarWidth.Default.Value)
                    .BindTo(this,
                        v=>v.SidebarGrid.Width),
                this.OneWayBind(ViewModel,
                    vm=>vm.CloseSidebarCommand,
                    v=>v.CloseSidebar.Command),

                this.OneWayBind(
                    ViewModel,
                    vm=>vm.NotificationOverlay,
                    v=>v.NotificationList.ViewModel),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.NavigationList,
                    v=>v.NavigationList.ViewModel),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.AppHeader,
                    v=>v.AppHeader.ViewModel),
            });
        }
    }
}
