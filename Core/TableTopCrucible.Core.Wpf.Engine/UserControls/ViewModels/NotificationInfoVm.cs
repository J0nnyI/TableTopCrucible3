using System;
using System.Reactive.Linq;
using System.Windows.Input;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Helper;
using TableTopCrucible.Core.Engine.Models;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Transient]
    public interface INotificationInfoVm
    {
        public void Init(NotificationInfo notification, bool initiallyExpanded);
        public NotificationInfo Notification { get; }
        bool TimerAlwaysVisible { get; set; }
    }
    public class NotificationInfoVm : ReactiveObject, INotificationInfoVm, IActivatableViewModel
    {

        public NotificationInfoVm(INotificationService notificationService)
        {
            this.WhenActivated(() => new[]
            {
                ReactiveCommandHelper.Create(
                    () => notificationService.RemoveNotification(Notification),
                    cmd => CloseNotificationCommand = cmd),

                this.WhenAnyObservable(vm=>vm.Notification.TimeRemainingChanges)
                    .Select(time=>(double)time.Value.Ticks)
                    .ToPropertyEx(this, vm=>vm.TicksRemaining,false,RxApp.MainThreadScheduler),

                this.WhenAnyValue(vm=>vm.Notification, vm=>vm.TimerAlwaysVisible,
                        (notification, timerAlwaysVisible)=>new{notification, timerAlwaysVisible})
                    .Select(x=>x.timerAlwaysVisible || x.notification.RemoveOnTimerComplete())
                    .ToPropertyEx(this, vm=>vm.ShowTimer, false, RxApp.MainThreadScheduler),

                this.WhenAnyValue(vm=>vm.IsExpanded, vm=>vm.Notification,
                        (expanded, notification)=>new{expanded,notification})
                    .Subscribe((x) =>
                    {
                        if(x.expanded)
                            x.notification.PauseCountdown();
                        else if(x.notification.IsCompleted is false)
                            x.notification.StartCountdown();
                    })
            });
        }
        [ObservableAsProperty]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public double TicksRemaining { get; }

        public double TicksTotal => SettingsHelper.NotificationAutoDeleteTime.Ticks;

        [ObservableAsProperty]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public bool ShowTimer { get; }
        public ICommand CloseNotificationCommand { get; private set; }

        public ViewModelActivator Activator { get; } = new();
        public NotificationId Id { get; } = NotificationId.New();

        public void Init(NotificationInfo notification, bool initiallyExpanded)
        {
            this.Notification = notification ?? throw new NullReferenceException(nameof(notification));
            IsExpanded = initiallyExpanded;
        }
        [Reactive]
        public bool IsExpanded { get; set; }
        [Reactive]
        public NotificationInfo Notification { get; set; }
        [Reactive]
        public bool TimerAlwaysVisible { get; set; }
    }
}