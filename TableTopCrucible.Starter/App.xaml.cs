using System.Windows;

using Splat;

using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine;
using TableTopCrucible.Domain.Library.Services;
using TableTopCrucible.Starter.Properties;

namespace TableTopCrucible.Starter;
/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        EngineStarter.InitializeEngine();
        Locator.Current.GetService<IFileWatcherService>()!.StartSynchronization();
        var srv = Locator.Current.GetService<ISettingsService>();

        //Locator.CurrentMutable.RegisterLazySingleton<ISettingsService>(()=>Settings.Default)
    }
}