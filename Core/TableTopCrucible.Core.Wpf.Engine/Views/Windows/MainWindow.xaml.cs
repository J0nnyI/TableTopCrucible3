using System.Windows;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Wpf.Engine.ViewModels.Windows;
using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.Views.Windows
{
    [Singleton(typeof(MainWindow))]
    internal interface IMainWindowStarter
    {
        public void Show();
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow :
        ReactiveWindow<MainWindowVm>,
        IMainWindowStarter,
        IViewFor<MainWindowVm>
    {
        public MainWindow()
        {
            this.DataContext = Locator.Current.GetService<IMainWindow>();
            InitializeComponent();
        }
    }
}
