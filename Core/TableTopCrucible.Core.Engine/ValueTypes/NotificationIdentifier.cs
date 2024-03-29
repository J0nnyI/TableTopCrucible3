﻿using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.ValueTypes
{
    /// <summary>
    ///     the readable notifier for a notification (i.e. Save Successful). this is used to detect duplicate notifications
    /// </summary>
    public class NotificationIdentifier : ValueType<string, NotificationIdentifier>
    {
    }
}