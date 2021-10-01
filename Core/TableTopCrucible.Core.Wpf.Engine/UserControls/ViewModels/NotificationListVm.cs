using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

using System;
using System.Reactive.Linq;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Transient(typeof(NotificationListVm))]
    public interface INotificationList
    {

    }
    public class NotificationListVm : ReactiveObject, INotificationList, IActivatableViewModel
    {
        private readonly ObservableCollectionExtended<INotification> _notificationList = new();
        public ObservableCollectionExtended<INotification> NotificationList => _notificationList;
        public NotificationListVm(INotificationService notificationService)
        {
            this.WhenActivated(() => new[]
            {
                notificationService.Notifications
                    .Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(_notificationList)
                    .Subscribe(),
            });
        }

        public ViewModelActivator Activator { get; } = new();
    }
}
