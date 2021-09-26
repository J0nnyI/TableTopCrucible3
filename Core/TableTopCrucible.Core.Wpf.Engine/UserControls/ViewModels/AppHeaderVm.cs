using ReactiveUI;

using System;
using System.Reactive.Linq;
using System.Windows.Shell;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
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
        private readonly IProgressTrackingService _progressService;
        public ViewModelActivator Activator { get; } = new();

        private ObservableAsPropertyHelper<string> _currentPageTitle;
        public string CurrentPageTitle => _currentPageTitle.Value;
        public IObservable<int> NotificationCountChanges { get; private set; }
        private ObservableAsPropertyHelper<CurrentProgressPercent> _globalJobProgress;
        public CurrentProgressPercent GlobalJobProgress => _globalJobProgress.Value;
        public IObservable<int> JobCountChanges => _progressService.TrackerList.CountChanged;

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
            INotificationService notificationService,
            IProgressTrackingService progressService)
        {
            _navigationService = navigationService;
            _notificationService = notificationService;
            _progressService = progressService;

            this.WhenActivated(() =>
            {
                this.NotificationCountChanges = _notificationService.Notifications.CountChanged.ObserveOn(RxApp.MainThreadScheduler);
                return new IDisposable[]
                {
                    this.WhenAnyValue(vm => vm._navigationService.CurrentPage.Title.Value)
                        .ToProperty(this, vm => vm.CurrentPageTitle, out _currentPageTitle),
                    this.WhenAnyValue(vm => vm._navigationService.IsSidebarExpanded)
                        .ToProperty(this, vm => vm.IsNavigationbarExpanded, out _isNavigationbarExpanded),
                    this._progressService.TotalProgress
                        .ObserveOn(RxApp.TaskpoolScheduler)
                        .ToProperty(this, vm=>vm.GlobalJobProgress, out _globalJobProgress),
                };
            });
        }
    }
}
