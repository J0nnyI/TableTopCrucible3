using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.Models
{
    /// <summary>
    /// contains information to display a notification
    /// </summary>
    public class SimpleNotification : INotification
    {
        public NotificationId Id { get; } = NotificationId.New();
        public Name Title { get; }
        public Description Content { get; }
        public NotificationType Type { get; }
        public NotificationIdentifier Identifier { get; }

        public SimpleNotification(Name title, Description content,NotificationType type, NotificationIdentifier identifier)
        {
            Title = title;
            Content = content;
            Type = type;
            Identifier = identifier;
        }
    }
}