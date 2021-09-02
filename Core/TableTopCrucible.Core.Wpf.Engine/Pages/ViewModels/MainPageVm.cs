using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels
{
    [Singleton(typeof(MainPageVm))]
    public interface IMainPage
    {

    }
    public class MainPageVm : IMainPage
    {
        public ISettingsPage SettingsPage { get; }

        public MainPageVm(ISettingsPage settingsPage)
        {
            SettingsPage = settingsPage;
        }
    }
}
