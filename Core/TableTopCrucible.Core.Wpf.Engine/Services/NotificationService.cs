using System;
using DynamicData;
using DynamicData.Binding;
using TableTopCrucible.Core.Engine.Models;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
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

        public NotificationId AddNotification(Name title, Description content, NotificationType type,
            NotificationIdentifier identifier = null)
        {
            var newNotification = new SimpleNotificationVm(title, content, type, identifier);

            if (identifier is not null)
                _notifications.RemoveWhere(notification => notification.Identifier == identifier);

            _notifications.AddOrUpdate(newNotification);
            return newNotification.Id;
        }

        public void RemoveNotification(NotificationId id)
            => _notifications.RemoveKey(id);
    }
}