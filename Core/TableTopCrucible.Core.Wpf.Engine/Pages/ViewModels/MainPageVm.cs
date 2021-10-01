using System;
using System.Linq.Expressions;
using System.Windows.Input;
using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels
{
    [Singleton(typeof(MainPageVm))]
    public interface IMainPage
    {

    }

    public class MainPageVm : ReactiveObject, IActivatableViewModel, IMainPage
    {
        private readonly INavigationService _navigationService;
        public INotificationList NotificationOverlay { get; }
        public INavigationList NavigationList { get; }
        public IAppHeader AppHeader { get; }

        public IObservable<INavigationPage> ActiveWorkareaChanges =>
            this.WhenAnyValue(vm => vm._navigationService.ActiveWorkarea);
        public IObservable<ISidebarPage> ActiveSidebarChanges =>
            this.WhenAnyValue(vm => vm._navigationService.ActiveSidebar);

        public ICommand CloseSidebarCommand { get; private set; }

        public MainPageVm(
            INotificationList notificationOverlay,
            INavigationList navigationList,
            INavigationService navigationService,
            IAppHeader appHeader)
        {
            _navigationService = navigationService;
            NotificationOverlay = notificationOverlay;
            NavigationList = navigationList;
            AppHeader = appHeader;

            this.WhenActivated(() => new []
            {
                ReactiveCommandHelper.Create(
                    ()=>navigationService.ActiveSidebar = null,
                    cmd=>this.CloseSidebarCommand = cmd)
            });
        }
        

        public ViewModelActivator Activator { get; } = new();
    }
}
