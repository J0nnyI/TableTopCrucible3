using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TableTopCrucible.Core.Engine.Models;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Engine.Helper;

public static class NotificationHelper
{
    public static IEnumerable<NotificationType> AutoClosingNotifications
        => JsonConvert.DeserializeObject<IEnumerable<string>>(SettingsHelper.AutoClosingNotifications)
            ?.Select(Enum.Parse<NotificationType>) ?? Enumerable.Empty<NotificationType>()
            .ToArray();

    public static bool RemoveOnTimerComplete(this INotification notification)
        => AutoClosingNotifications.Contains(notification.Type);
}