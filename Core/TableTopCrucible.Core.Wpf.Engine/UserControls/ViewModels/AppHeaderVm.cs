using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Services;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Singleton(typeof(AppHeaderVm))]
    public interface IAppHeader
    {

    }
    public class AppHeaderVm : ReactiveObject, IActivatableViewModel, IAppHeader
    {
        private readonly INavigationService _navigationService;
        private readonly INotificationService _notificationService;
        public ViewModelActivator Activator { get; } = new();

        private ObservableAsPropertyHelper<string> _currentPageTitle;
        public string CurrentPageTitle => _currentPageTitle.Value;
        public IObservable<int> NotificationCountChanges { get; private set; }

        private ObservableAsPropertyHelper<bool> _isNavigationbarExpanded;
        public bool IsNavigationbarExpanded
        {
            get => _isNavigationbarExpanded.Value;
            set
            {
                if (_navigationService.IsSidebarExpanded != value)
                    _navigationService.IsSidebarExpanded = value;
            }
        }

        public AppHeaderVm(
            INavigationService navigationService,
            INotificationService notificationService)
        {
            _navigationService = navigationService;
            _notificationService = notificationService;
            this.WhenActivated(() =>
            {
                this.NotificationCountChanges = _notificationService.Notifications.CountChanged.ObserveOn(RxApp.MainThreadScheduler);
                return new IDisposable[]
                {
                    this.WhenAnyValue(vm => vm._navigationService.CurrentPage.Title.Value)
                        .ToProperty(this, vm => vm.CurrentPageTitle, out _currentPageTitle),
                    this.WhenAnyValue(vm => vm._navigationService.IsSidebarExpanded)
                        .ToProperty(this, vm => vm.IsNavigationbarExpanded, out _isNavigationbarExpanded),
                };
            });
        }
    }
}
