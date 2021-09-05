using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton(typeof(NotificationService))]
    public interface INotificationService
    {
        NotificationId AddNotification(Name title, Description content, NotificationType type);
        void RemoveNotification(NotificationId id);
    }
    public class NotificationService : INotificationService
    {
        private readonly SourceCache<INotification, NotificationId> _notifications = new(n => n.Id);
        public IObservableList<INotification> Notifications { get; }

        public NotificationService()
        {
            this._notifications
                .Connect()
                .Sort(n => n.Timestamp)
                .BindToObservableList(out var observableList)
                .Subscribe();
            this.Notifications = observableList;

        }
        public NotificationId AddNotification(Name title, Description content, NotificationType type)
        {
            var notification = new SimpleNotificationVm(title, content, type);
            _notifications.AddOrUpdate(notification);
            if (type == NotificationType.Confirmation && SettingsHelper.AutocloseEnabled)
            {
                Observable.Start(() => _notifications.RemoveKey(notification.Id), RxApp.TaskpoolScheduler)
                    .Take(1)
                    .DelaySubscription(SettingsHelper.NotificationDelay)
                    .Subscribe();
            }
            return notification.Id;
        }

        public void RemoveNotification(NotificationId id)
            => _notifications.RemoveKey(id);
    }
}
