using ReactiveUI;

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using TableTopCrucible.DomainCore.WPF.Startup.PageViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.PageViews
{
    /// <summary>
    /// Interaction logic for StartupPageV.xaml
    /// </summary>
    public partial class StartupPageV : UserControl, IViewFor<StartupPageVM>
    {
        public StartupPageV()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                createNewFile
                    .Events()
                    .MouseDown
                    .Select(_ => Unit.Default)
                    .InvokeCommand(ViewModel.OpenDirectoryWizard)
                    .DisposeWith(disposables);
            });
        }

        public StartupPageVM ViewModel { get => DataContext as StartupPageVM; set => DataContext = value; }
        object IViewFor.ViewModel { get => DataContext; set => DataContext = value; }
    }
}
