using System;
using TableTopCrucible.Core.Engine.ValueTypes;

namespace TableTopCrucible.Core.Engine.Models
{
    public interface INotification
    {
        public NotificationId Id { get; }
        public DateTime Timestamp { get; }
        public NotificationType Type { get; }
        NotificationIdentifier Identifier { get; init; }
    }
}