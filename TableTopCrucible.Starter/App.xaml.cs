
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ReactiveUI;

using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

using System.Linq;
using System.Windows;

using TableTopCrucible.Core.Wpf.Engine;
using TableTopCrucible.Core.Wpf.Engine.Windows.Views;

namespace TableTopCrucible.App.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            EngineStarter.InitializeEngine();
        }
    }
}
