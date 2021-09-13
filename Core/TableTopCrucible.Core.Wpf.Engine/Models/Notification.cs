using System;

using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Models
{
    public interface INotification
    {
        public NotificationId Id { get; }
        public DateTime Timestamp { get; }
        public NotificationType Type { get; }
    }
}
