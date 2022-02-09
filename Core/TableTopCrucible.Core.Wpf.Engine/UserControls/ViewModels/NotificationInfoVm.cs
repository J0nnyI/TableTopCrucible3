using System;
using System.Reactive.Linq;
using System.Runtime.InteropServices.ComTypes;
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
        public void Init(NotificationInfo notification);
        public NotificationInfo Notification { get; }
        bool TimerAlwaysVisible { get; set; }
        bool ProvideClose { get; set; }
    }
    public class NotificationInfoVm : ReactiveObject, INotificationInfoVm, IActivatableViewModel
    {

        public NotificationInfoVm(INotificationService notificationService)
        {
            this.WhenActivated(() => new IDisposable[]
            {
                ReactiveCommandHelper.Create(
                    () => notificationService.RemoveNotification(Notification),
                    cmd => CloseNotificationCommand = cmd),

                this.WhenAnyObservable(vm=>vm.Notification.TimeRemainingChanges)
                    .Select(time=>(double)time.Value.Ticks)
                    .ToPropertyEx(this, vm=>vm.TicksRemaining,false,RxApp.MainThreadScheduler),

                this.WhenAnyValue(vm=>vm.Notification, vm=>vm.Notification.CountdownRunning, vm=>vm.TimerAlwaysVisible,
                        (notification,running, timerAlwaysVisible)=>
                            running 
                            && (timerAlwaysVisible 
                                || notification.RemoveOnTimerComplete()))
                    .ToPropertyEx(this, vm=>vm.ShowTimer, false, RxApp.MainThreadScheduler),

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

        public void Init(NotificationInfo notification)
        {
            this.Notification = notification ?? throw new NullReferenceException(nameof(notification));
        }
        [Reactive]
        public NotificationInfo Notification { get; set; }
        [Reactive]
        public bool TimerAlwaysVisible { get; set; }
        [Reactive]
        public bool ProvideClose { get; set; }
    }
}