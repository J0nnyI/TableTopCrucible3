using System;

namespace TableTopCrucible.Core.Helper
{
    // todo temporary class until a proper settings repository is implemented
    public static class SettingsHelper
    {
        public static TimeSpan NotificationDelay => new(0, 0, 0, 5);
        public static double NotificationResolution => NotificationDelay / AnimationResolution;
        public static bool AutocloseEnabled => true;
        public static double AnimationFrames => AnimationDuration / AnimationResolution;
        public static TimeSpan AnimationResolution => new TimeSpan(0, 0, 0, 1) / 30;
        public static TimeSpan AnimationDuration => new(0, 0, 0, 1);
    }
}
