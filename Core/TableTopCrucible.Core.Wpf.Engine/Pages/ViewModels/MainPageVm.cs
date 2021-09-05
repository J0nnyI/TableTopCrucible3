using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels
{
    [Singleton(typeof(MainPageVm))]
    public interface IMainPage
    {

    }
    public class MainPageVm : IMainPage
    {
        public ISettingsPage SettingsPage { get; }
        public IBannerList BannerList { get; }

        public MainPageVm(ISettingsPage settingsPage, IBannerList bannerList)
        {
            SettingsPage = settingsPage;
            BannerList = bannerList;
        }
    }
}
