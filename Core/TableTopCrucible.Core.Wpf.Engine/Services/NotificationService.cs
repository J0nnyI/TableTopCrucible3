using System;
using DynamicData;
using DynamicData.Binding;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton]
    public interface INotificationService
    {
        IObservableList<INotification> Notifications { get; }
        NotificationId AddNotification(Name title, Description content, NotificationType type);
        void RemoveNotification(NotificationId id);
    }

    public class NotificationService : INotificationService
    {
        private readonly SourceCache<INotification, NotificationId> _notifications = new(n => n.Id);

        public NotificationService()
        {
            _notifications
                .Connect()
                .Sort(n => n.Timestamp)
                .BindToObservableList(out var observableList)
                .Subscribe();
            Notifications = observableList;
        }

        public IObservableList<INotification> Notifications { get; }

        public NotificationId AddNotification(Name title, Description content, NotificationType type)
        {
            var notification = new SimpleNotificationVm(title, content, type);
            _notifications.AddOrUpdate(notification);
            return notification.Id;
        }

        public void RemoveNotification(NotificationId id)
            => _notifications.RemoveKey(id);
    }
}