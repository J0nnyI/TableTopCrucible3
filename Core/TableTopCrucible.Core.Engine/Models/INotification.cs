using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.Models;

/// <summary>
/// base interface for all notifications
/// </summary>
public interface INotification
{
    NotificationId Id { get; }
    NotificationType Type { get; }
    NotificationIdentifier Identifier { get; }
    Name Title { get; }
    Description Content { get; }
}