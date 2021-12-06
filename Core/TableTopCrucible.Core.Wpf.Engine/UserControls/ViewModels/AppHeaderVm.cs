using ReactiveUI;

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Shell;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Infrastructure.DataPersistence;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Singleton]
    public interface IAppHeader
    {

    }
    public class AppHeaderVm : ReactiveObject, IActivatableViewModel, IAppHeader
    {
        private readonly INavigationService _navigationService;
        private readonly IProgressTrackingService _progressService;
        public ViewModelActivator Activator { get; } = new();

        public IObservable<Name> CurrentPageTitleChanges { get; }
        public IObservable<int> NotificationCountChanges { get; private set; }
        public IObservable<CurrentProgressPercent> GlobalJobProgressChanges => _progressService.TotalProgress;
        public IObservable<int> JobCountChanges =>
            _progressService
                .TrackerList
                .Connect()
                .ToCollection()
                .Select(jobs => jobs
                    .Select(job => job.JobStateChanges).CombineLatest())
                .Switch()
                .Select(states => states.Count(state => state != JobState.Done));

        private ObservableAsPropertyHelper<bool> _isNavigationBarExpanded;
        public bool IsNavigationBarExpanded
        {
            get => _isNavigationBarExpanded.Value;
            set
            {
                if (_navigationService.IsNavigationExpanded != value)
                    _navigationService.IsNavigationExpanded = value;
            }
        }

        public IObservable<bool> IsNotificationSidebarSelectedChanged { get; private set; }
        public IObservable<bool> IsJobQueueSelectedChanged { get; private set; }

        public ICommand ShowJobSidebarCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ShowNotificationSidebar { get; private set; }

        public AppHeaderVm(
            INavigationService navigationService,
            INotificationService notificationService,
            IProgressTrackingService progressService,
            INotificationList notificationList,
            IJobQueuePage jobQueuePage,
            IDatabaseAccessor database)
        {
            _navigationService = navigationService;
            _progressService = progressService;
            CurrentPageTitleChanges = this.WhenAnyValue(vm => vm._navigationService.ActiveWorkArea.Title);

            this.WhenActivated(() =>
            {
                this.NotificationCountChanges = notificationService.Notifications.CountChanged.ObserveOn(RxApp.MainThreadScheduler);

                this.IsNotificationSidebarSelectedChanged =
                    this.WhenAnyValue(vm => vm._navigationService.ActiveSidebar)
                    .Select(sidebar => sidebar == notificationList);
                this.IsJobQueueSelectedChanged =
                    this.WhenAnyValue(vm => vm._navigationService.ActiveSidebar)
                        .Select(sidebar => sidebar == jobQueuePage);

                return new IDisposable[]
                {
                    this.WhenAnyValue(vm => vm._navigationService.IsNavigationExpanded)
                        .ToProperty(this, vm => vm.IsNavigationBarExpanded, out _isNavigationBarExpanded),
                    ReactiveCommandHelper.Create(() =>
                        {
                            _navigationService.ActiveSidebar =
                                _navigationService.ActiveSidebar == notificationList
                                    ? null
                                    : notificationList;
                        },
                        cmd=>ShowNotificationSidebar = cmd),
                    ReactiveCommandHelper.Create(()=>
                        {
                            _navigationService.ActiveSidebar =
                                _navigationService.ActiveSidebar == jobQueuePage
                                    ? null
                                    : jobQueuePage;
                        },
                        cmd=>ShowJobSidebarCommand = cmd),
                    ReactiveCommandHelper.Create(()=>database.ManualSave(),
                        cmd=>SaveCommand = cmd)
                };
            });
        }
    }
}
