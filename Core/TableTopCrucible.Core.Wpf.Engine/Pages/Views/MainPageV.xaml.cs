using ReactiveUI;

using System.Reactive.Disposables;

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

            this.WhenActivated(()=>new[]
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.SettingsPage,
                        v => v.MainContainer.ViewModel),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.BannerList,
                    v=>v.NotificationList.ViewModel),
            });
        }
    }
}
