using System;
using System.IO;
using System.Windows;

namespace TableTopCrucible.Core.Helper
{
    // todo temporary class until a proper settings repository is implemented
    public static class SettingsHelper
    {
        public static bool AutoSaveEnabled = true;
        public static TimeSpan AutoSaveThrottle = new(0,0,0,30);
        public static TimeSpan NotificationDelay => new(0, 0, 0, 5);
        public static int FileMinLoadingScreenSize => 500000;
        public static double NotificationResolution => NotificationDelay / AnimationResolution;

        public static TimeSpan AutoFileScanThrottle => TimeSpan.FromSeconds(5);

        //notifications
        public static bool AutoCloseEnabled => true;
        public static double AnimationFrames => AnimationDuration / AnimationResolution;
        public static TimeSpan AnimationResolution => new TimeSpan(0, 0, 0, 1) / 10;
        public static TimeSpan AnimationDuration => TimeSpan.FromMilliseconds(200);

        public static int ThreadCount => 2;
        public static ushort SimultaneousThumbnailWindows => 3;

        public static TimeSpan PipelineBufferTime => TimeSpan.FromSeconds(5);

        // the last n jobs will stay in memory, everything else will be removed
        public static int DoneJobLimit => 5;
        public static Size ThumbnailSize = new(200, 200);
        public static double ThumbnailHeight => ThumbnailSize.Height;
        public static double ThumbnailWidth=> ThumbnailSize.Width;
        public static bool GenerateThumbnailOnSync => false;

        /// <summary>
        ///     the autoSave (subject) is buffered by this duration to prevent excessive writing
        /// </summary>
        public static TimeSpan AutoSaveBuffer => TimeSpan.FromMinutes(1);

        public static string DefaultFilePath =>
            //"library.ttcl";
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TableTopCrucible",
                "Library.ttcl");
    }
}