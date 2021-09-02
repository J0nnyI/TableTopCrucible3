using System.Windows;

using TableTopCrucible.Core.Wpf.Engine;

namespace TableTopCrucible.Starter
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
