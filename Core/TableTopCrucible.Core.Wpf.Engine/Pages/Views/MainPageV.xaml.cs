using ReactiveUI;

using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;

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

            this.WhenActivated(() => new[]
            {
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.NotificationList,
                    v=>v.NotificationList.ViewModel),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.NavigationList,
                    v=>v.NavigationList.ViewModel),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.CurrentPage,
                    v=>v.MainContainer.ViewModel),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.AppHeader,
                    v=>v.AppHeader.ViewModel),
            });
        }
    }
}
