﻿using System;

namespace TableTopCrucible.Core.Helper
{
    // todo temporary class until a proper settings repository is implemented
    public static class SettingsHelper
    {
        public static TimeSpan NotificationDelay => new(0, 0, 0, 5);
        public static double NotificationResolution => NotificationDelay / AnimationResolution;
        public static bool AutoCloseEnabled => true;
        public static double AnimationFrames => AnimationDuration / AnimationResolution;
        public static TimeSpan AnimationResolution => new TimeSpan(0, 0, 0, 1) / 10;
        public static TimeSpan AnimationDuration => TimeSpan.FromMilliseconds(200);

        public static int ThreadCount => 2;
        public static TimeSpan PipelineBufferTime => TimeSpan.FromSeconds(5);
        // the last n jobs will stay in memory, everything else will be removed
        public static int DoneJobLimit => 5;
    }
}