using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

using System;
using System.Reactive.Linq;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    /// <summary>
    /// todo: add a proper page
    /// </summary>
    [Transient(typeof(NotificationListVm))]
    public interface INotificationList:ISidebarPage
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
        public Name Title => (Name) "Notifications";
        public SidebarWidth Width => null;
    }
}
