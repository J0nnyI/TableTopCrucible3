using System.Reactive.Disposables;
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
        IMainWindowStarter
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.ViewModel = (MainWindowVm)Locator.Current.GetService<IMainWindow>();
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.OneWayBind(
                    ViewModel, 
                    vm => vm.SettingsPage, 
                    v => v.MainContainer.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.SettingsPage,
                        v => v.TestLabel.Content)
                    .DisposeWith(disposables);
            });
        }

    }
}
