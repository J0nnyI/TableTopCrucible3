using System;
using System.Reactive.Linq;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Helper;
using TableTopCrucible.Core.Engine.Models;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.Services;

[Singleton]
public interface INotificationService
{
    IObservableList<NotificationInfo> Notifications { get; }

    void RemoveNotification(NotificationId id);
    void RemoveNotification(INotification notification);

    NotificationId AddNotification(Name title, Description content, NotificationType type,
        NotificationIdentifier identifier = null);

    NotificationId AddInfo(Name title, Description content, NotificationIdentifier identifier = null);
    NotificationId AddConfirmation(Name title, Description content, NotificationIdentifier identifier = null);
    NotificationId AddWarning(Name title, Description content, NotificationIdentifier identifier = null);

    NotificationId AddWarning(Exception exception, Name title = null, Description content = null,
        NotificationIdentifier identifier = null);

    NotificationId AddError(Name title, Description content, NotificationIdentifier identifier = null,
        Exception exception = null);

    NotificationId AddError(Exception exception, Name title = null, Description content = null,
        NotificationIdentifier identifier = null);
}

// ReSharper disable once UnusedMember.Global
public class NotificationService : INotificationService
{
    private readonly SourceCache<NotificationInfo, NotificationId> _notifications = new(n => n.Id);

    public NotificationService()
    {
        Notifications = _notifications
            .Connect()
            .DisposeMany()
            .RemoveKey()
            .AsObservableList();

        _notifications.Connect()
            .OnItemAdded(notification =>
            {
                if (notification.RemoveOnTimerComplete())
                {
                    notification.IsCompletedChanges
                        .Where(x => x)
                        .Take(1)
                        .Subscribe(_ => this.RemoveNotification(notification));
                }
            })
            .Subscribe();
    }

    public IObservableList<NotificationInfo> Notifications { get; }

    #region add methods

    public NotificationId AddNotification(
        Name title,
        Description content,
        NotificationType type,
        NotificationIdentifier identifier = null)
        => _addNotification(new SimpleNotification(title, content, type, identifier));

    public NotificationId AddInfo(
        Name title,
        Description content,
        NotificationIdentifier identifier = null)
        => _addNotification(new SimpleNotification(title, content, NotificationType.Info, identifier));

    public NotificationId AddConfirmation(
        Name title,
        Description content,
        NotificationIdentifier identifier = null)
        => _addNotification(new SimpleNotification(title, content, NotificationType.Confirmation, identifier));

    public NotificationId AddWarning(
        Name title,
        Description content,
        NotificationIdentifier identifier = null)
        => _addNotification(new SimpleNotification(title, content, NotificationType.Warning, identifier));

    public NotificationId AddWarning(
        Exception exception,
        Name title = null,
        Description content = null,
        NotificationIdentifier identifier = null)
        => _addNotification(new ExceptionNotification(exception, title, content, NotificationType.Warning,
            identifier));

    public NotificationId AddError(
        Name title,
        Description content,
        NotificationIdentifier identifier = null,
        Exception exception = null)
        => _addNotification(new SimpleNotification(title, content, NotificationType.Error, identifier));

    public NotificationId AddError(
        Exception exception,
        Name title = null,
        Description content = null,
        NotificationIdentifier identifier = null)
        => _addNotification(
            new ExceptionNotification(exception, title, content, NotificationType.Error, identifier));

    private NotificationId _addNotification(INotification notification)
    {
        var newNotification = new NotificationInfo(notification);

        _notifications.Edit(updater =>
        {
            if (notification.Identifier is not null)
                updater.RemoveWhere(curNotification => curNotification.Identifier == notification.Identifier);

            updater.AddOrUpdate(newNotification);
        });

        return newNotification.Id;
    }

    #endregion

    public void RemoveNotification(NotificationId id)
        => _notifications.RemoveKey(id);

    public void RemoveNotification(INotification notification)
        => RemoveNotification(notification.Id);
}