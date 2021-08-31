using ReactiveUI;
using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;
using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.Windows.ViewModels
{
    [Singleton(typeof(MainWindowVm))]
    public interface IMainWindow
    {
        string TestValue { get; set; }

    }
    internal class MainWindowVm: IMainWindow, IActivatableViewModel
    {
        public ISettingsPage SettingsPage { get; }
        public string TestValue { get; set; } = "works";
        
        public MainWindowVm(ISettingsPage settingsPage)
        {
            SettingsPage = settingsPage;
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
