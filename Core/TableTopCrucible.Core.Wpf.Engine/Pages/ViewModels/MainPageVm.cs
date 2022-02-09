using System;
using System.Windows.Input;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels
{
    [Singleton]
    public interface IMainPage
    {
    }

    public class MainPageVm : ReactiveObject, IActivatableViewModel, IMainPage
    {
        private readonly INavigationService _navigationService;

        public MainPageVm(
            INotificationList notificationOverlay,
            INavigationList navigationList,
            INavigationService navigationService,
            IAppHeader appHeader)
        {
            _navigationService = navigationService;
            NotificationOverlay = notificationOverlay;
            NotificationOverlay.ShowCompleted = false;
            NotificationOverlay.ProvideClose = false;
            NavigationList = navigationList;
            AppHeader = appHeader;

            this.WhenActivated(() => new[]
            {
                ReactiveCommandHelper.Create(
                    () => navigationService.ActiveSidebar = null,
                    cmd => CloseSidebarCommand = cmd)
            });
        }

        public INotificationList NotificationOverlay { get; }
        public INavigationList NavigationList { get; }
        public IAppHeader AppHeader { get; }

        public IObservable<INavigationPage> ActiveWorkAreaChanges =>
            this.WhenAnyValue(vm => vm._navigationService.ActiveWorkArea);

        public IObservable<ISidebarPage> ActiveSidebarChanges =>
            this.WhenAnyValue(vm => vm._navigationService.ActiveSidebar);

        public ICommand CloseSidebarCommand { get; private set; }


        public ViewModelActivator Activator { get; } = new();
    }
}