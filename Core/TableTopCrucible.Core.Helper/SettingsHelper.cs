using System;
using System.IO;

namespace TableTopCrucible.Core.Helper
{
    // todo temporary class until a proper settings repository is implemented
    public static class SettingsHelper
    {
        public static TimeSpan NotificationDelay => new(0, 0, 0, 5);
        public static double NotificationResolution => NotificationDelay / AnimationResolution;
        //notifications
        public static bool AutoCloseEnabled => true;
        public static double AnimationFrames => AnimationDuration / AnimationResolution;
        public static TimeSpan AnimationResolution => new TimeSpan(0, 0, 0, 1) / 10;
        public static TimeSpan AnimationDuration => TimeSpan.FromMilliseconds(200);

        public static int ThreadCount => 2;
        public static TimeSpan PipelineBufferTime => TimeSpan.FromSeconds(5);
        // the last n jobs will stay in memory, everything else will be removed
        public static int DoneJobLimit => 5;
        /// <summary>
        /// the autoSave (subject) is buffered by this duration to prevent excessive writing
        /// </summary>
        public static TimeSpan AutoSaveBuffer => TimeSpan.FromMinutes(1);

        public static bool AutoSaveEnabled = true;

        public static string DefaultFilePath =>
            //"library.ttcl";
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TableTopCrucible", "Library.ttcl");
    }

}