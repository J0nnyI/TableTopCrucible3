using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using Splat;
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
