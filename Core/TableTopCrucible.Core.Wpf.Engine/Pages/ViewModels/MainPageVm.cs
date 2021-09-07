using System;
using System.Reactive.Linq;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels
{
    [Singleton(typeof(MainPageVm))]
    public interface IMainPage
    {

    }
    public class MainPageVm : ReactiveObject, IActivatableViewModel, IMainPage
    {
        private readonly INavigationService _navigationService;
        public IBannerList BannerList { get; }
        public INavigationList NavigationList { get; }

        private ObservableAsPropertyHelper<INavigationPage> _currentPage;
        public INavigationPage CurrentPage => _currentPage.Value;

        public MainPageVm( IBannerList bannerList, INavigationList navigationList, INavigationService navigationService)
        {
            _navigationService = navigationService;
            BannerList = bannerList;
            NavigationList = navigationList;

            this.WhenActivated(()=> new []
            {
                this.WhenAnyValue(
                    vm=>vm._navigationService.CurrentPage)
                    .ToProperty(
                        this,
                        vm=>vm.CurrentPage,
                        out _currentPage)
            });
        }

        public ViewModelActivator Activator { get; } = new();
    }
}
