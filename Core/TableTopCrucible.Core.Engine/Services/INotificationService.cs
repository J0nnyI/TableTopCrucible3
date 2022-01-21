using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Models;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.Services
{
    [Singleton]
    public interface INotificationService
    {
        IObservableList<INotification> Notifications { get; }
        NotificationId AddNotification(Name title, Description content, NotificationType type, NotificationIdentifier identifier = null);
        void RemoveNotification(NotificationId id);
    }
}