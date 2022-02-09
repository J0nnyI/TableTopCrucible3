using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using TableTopCrucible.Core.DependencyInjection;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    /// <summary>
    ///     todo: add a proper page
    /// </summary>
    [Transient]
    public interface INotificationList : ISidebarPage
    {
        bool ShowCompleted { get; set; }
    }

    public class NotificationListVm : ReactiveObject, INotificationList, IActivatableViewModel
    {
        public NotificationListVm(INotificationService notificationService)
        {
            var showCompletedChanges =
                this.WhenAnyValue(vm => vm.ShowCompleted)
                    .Replay(1).RefCount();
            this.WhenActivated(() => new[]
            {
                /*      ui                          |           
                 * showCompleted        complete    |   show    
                 * 0                    0           |   1       
                 * 0                      1         |     0     
                 *   1                  0           |   1       
                 *   1                    1         |     1     
                 */
                notificationService.Notifications
                    .Connect()
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Transform(notification =>
                    {
                        var vm = Locator.Current.GetRequiredService<INotificationInfoVm>();
                        vm.Init(notification, InitiallyExpanded);
                        vm.TimerAlwaysVisible = ShowCompleted is false;// might cause issues when the property is set after activation-but that should never happen
                        return vm;
                    })
                    .FilterOnObservable(vm=>
                        vm.Notification.IsCompletedChanges
                            .CombineLatest(showCompletedChanges,
                                (complete, showCompleted)=>
                                    showCompleted || !complete),
                        null,RxApp.TaskpoolScheduler)
                    .Sort(n => n.Notification.Created)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _notificationList)
                    .Subscribe()
            });
        }

        private ReadOnlyObservableCollection<INotificationInfoVm> _notificationList;
        public ReadOnlyObservableCollection<INotificationInfoVm> NotificationList => _notificationList;

        public ViewModelActivator Activator { get; } = new();
        public Name Title => "Notifications";
        public SidebarWidth Width => null;
        [Reactive]
        public bool InitiallyExpanded { get; set; } = false;
        [Reactive]
        public bool ShowCompleted { get; set; } = true;
    }
}