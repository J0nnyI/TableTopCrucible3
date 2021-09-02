using ReactiveUI;

using System.Reactive.Disposables;

using TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.Views
{
    /// <summary>
    /// Interaction logic for MainPageV.xaml
    /// </summary>
    public partial class MainPageV : ReactiveUserControl<MainPageVm>
    {
        public MainPageV()
        {
            InitializeComponent();

            this.InitializeComponent();
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.SettingsPage,
                        v => v.MainContainer.ViewModel)
                    .DisposeWith(disposables);
            });
        }
    }
}
