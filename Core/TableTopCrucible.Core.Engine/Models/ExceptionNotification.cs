using System;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.Models
{
    public class ExceptionNotification : INotification
    {
        public ExceptionNotification(Exception exception, Name title=null, Description content=null, NotificationType type = NotificationType.Error, NotificationIdentifier identifier = null)
        {
            Type = type;
            Identifier = identifier;
            Exception = exception;
            Title = title ?? "Operation failed";
            Content = content ?? "Error: " + Environment.NewLine + exception;
        }

        public NotificationId Id { get; } = NotificationId.New();
        public NotificationType Type { get; }
        public NotificationIdentifier Identifier { get; }
        public Exception Exception { get; }
        public Name Title { get; }
        public Description Content { get; }
    }
}