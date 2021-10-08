using Splat;

using System.Windows;

using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels;
using TableTopCrucible.Domain.Settings.Wpf.Pages.ViewModels;
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
                navigationService.ActiveWorkArea = Locator.Current.GetService<IDirectorySetupPage>();
            else
                navigationService.ActiveWorkArea = Locator.Current.GetService<ILibraryPage>();
        }
    }
}
