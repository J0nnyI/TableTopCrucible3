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
        public IBannerList BannerList { get; }
        public INavigationList NavigationList { get; }

        public MainPageVm( IBannerList bannerList, INavigationList navigationList)
        {
            BannerList = bannerList;
            NavigationList = navigationList;
        }
    }
}
