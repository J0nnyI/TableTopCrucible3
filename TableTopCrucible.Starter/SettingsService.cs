
using ReactiveUI;
using ReactiveUI.Wpf;

using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Starter.Properties;

namespace TableTopCrucible.Starter;

internal class SettingsService : ISettingsService
{
    public SettingsService()
    {

    }
    private abstract class SettingsCategory : ReactiveObject
    {
        protected void UpdateProperty<T>(T value,
            ref T field,
            Action settingsUpdater,
            [CallerMemberName] string propertyName = "")
        {
            if (value.Equals(field))
                return;

            field = value;
            settingsUpdater();
            this.RaisePropertyChanged(nameof(propertyName));
        }
    }
    private class StlThumbSettings : SettingsCategory, IStlThumbSettings
    {
        private ExecutableFilePath _installationPath = (ExecutableFilePath)Settings.Default.stl_thumb_path;
        public ExecutableFilePath InstallationPath
        {
            get => _installationPath;
            set => UpdateProperty(value,
                ref _installationPath,
                () => Settings.Default.stl_thumb_path = value.Value);
        }
        public Color _defaultMaterial = Settings.Default.stl_thumb_material.ToMediaColor();
        public Color DefaultMaterial
        {
            get => _defaultMaterial;
            set => UpdateProperty(value,
                ref _defaultMaterial,
                () => Settings.Default.stl_thumb_material = value.ToDrawingColor());
        }
        public bool _useStlThumb = Settings.Default.use_stl_thumb;
        public bool Enabled
        {
            get => _useStlThumb;
            set => UpdateProperty(value,
                ref _useStlThumb,
                () => Settings.Default.use_stl_thumb = value);
        }
        public int _timeOut = Settings.Default.stlThumbTimeOut;
        public int TimeOut
        {
            get => _timeOut;
            set => UpdateProperty(value,
                ref _timeOut,
                () => Settings.Default.stlThumbTimeOut = value);
        }
    }
    public IStlThumbSettings StlThumb { get; } = new StlThumbSettings();
    private class SynchronizationSettings : SettingsCategory, ISynchronizationSettings
    {
        private bool _autoGenerateThumbnail;
        public bool AutoGenerateThumbnail
        {
            get => _autoGenerateThumbnail;
            set=> UpdateProperty(value,ref _autoGenerateThumbnail,()=>Settings.Default.sync_autoThumbnails = value);
        }
    }
    public ISynchronizationSettings Synchronization { get; } = new SynchronizationSettings();
}
