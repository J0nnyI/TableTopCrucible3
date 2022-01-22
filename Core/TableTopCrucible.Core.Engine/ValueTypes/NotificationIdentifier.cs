using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.ValueTypes
{
    /// <summary>
    /// the readable notifier for a notification (i.e. Save Successful). this is used to detect duplicate notifications
    /// </summary>
    public class NotificationIdentifier : ValueType<string, NotificationIdentifier>
    {
    }
}
