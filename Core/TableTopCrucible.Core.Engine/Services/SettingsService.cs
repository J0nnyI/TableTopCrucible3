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
    [Reactive]
    public ExecutableFilePath InstallationPath { get; set; }
    [Reactive]
    public Color DefaultMaterial { get; set; }
    [Reactive]
    public bool Enabled { get; set; }
    [Reactive]
    public int TimeOut{ get; set; }
}

[Singleton]
public interface ISettingsService
{
    public IStlThumbSettings StlThumb { get; }
}
