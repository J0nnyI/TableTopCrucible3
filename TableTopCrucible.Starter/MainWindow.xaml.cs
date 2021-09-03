using Splat;

using System.Windows;

using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

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

        }
    }
}
