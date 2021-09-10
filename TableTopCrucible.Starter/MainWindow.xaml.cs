using System.Reactive.Linq;
using Splat;

using System.Windows;

using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels;
using TableTopCrucible.Domain.Settings.Wpf.PageViewModels;
using TableTopCrucible.Infrastructure.Repositories;

namespace TableTopCrucible.Starter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MainPageContainer.ViewModel = Locator.Current.GetService<IMainPage>();


            var navigationService = Locator.Current.GetService<INavigationService>();
            var directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>();
            
            if (directorySetupRepository.Data.Count == 0)
                navigationService.CurrentPage = Locator.Current.GetService<IDirectorySetupPage>();
            else
                navigationService.CurrentPage = Locator.Current.GetService<ILibraryPage>();
        }
    }
}
