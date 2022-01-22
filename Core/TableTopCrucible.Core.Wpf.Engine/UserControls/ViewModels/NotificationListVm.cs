using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Models;
using TableTopCrucible.Core.Engine.Services;
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
    }

    public class NotificationListVm : ReactiveObject, INotificationList, IActivatableViewModel
    {
        public NotificationListVm(INotificationService notificationService)
        {
            this.WhenActivated(() => new[]
            {
                notificationService.Notifications
                    .Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(NotificationList)
                    .Subscribe()
            });
        }

        public ObservableCollectionExtended<INotification> NotificationList { get; } = new();

        public ViewModelActivator Activator { get; } = new();
        public Name Title => (Name)"Notifications";
        public SidebarWidth Width => null;
    }
}