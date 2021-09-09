using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.Helper
{
    // todo temporary class until a proper settings repository is implemented
    public static class SettingsHelper
    {
        public static TimeSpan NotificationDelay => new (0, 0, 0, 5);
        public static int NotificationResolution => 100;
        public static bool AutocloseEnabled => true;
    }
}
