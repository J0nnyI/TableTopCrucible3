using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.Services;

public interface IStlThumbSettings : IReactiveObject
{
    public ExecutableFilePath InstallationPath { get; set; }
    public Color DefaultMaterial { get; set; }
    public bool Enabled { get; set; }
    public int TimeOut{ get; set; }
}

public interface ISynchronizationSettings : IReactiveObject
{
    public bool AutoGenerateThumbnail { get; set; }
}

[Singleton]
public interface ISettingsService
{
    public IStlThumbSettings StlThumb { get; }
    public ISynchronizationSettings Synchronization { get; }
}
