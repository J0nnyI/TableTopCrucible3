using System;
using System.Linq;
using System.Windows;
using Splat;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels;
using TableTopCrucible.Domain.Settings.Wpf.Pages.ViewModels;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Starter;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainPageContainer.ViewModel = Locator.Current.GetService<IMainPage>();


        var navigationService = Locator.Current.GetService<INavigationService>()!;
        var directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>()!;
        try
        {
#if DEBUG&&false
            navigationService.ActiveWorkArea = Locator.Current.GetService<IDevAssist>();
                return;
#else
            if (directorySetupRepository.Data.Items.Any())
                navigationService.ActiveWorkArea = Locator.Current.GetService<ILibraryPage>();
            else
                navigationService.ActiveWorkArea = Locator.Current.GetService<IDirectorySetupPage>();
#endif
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}