using ReactiveUI;

using System.Reactive.Disposables;
using System.Windows.Controls;

using TableTopCrucible.App.WpfTestApp.ViewModels;

namespace TableTopCrucible.App.WpfTestApp.Views
{
    /// <summary>
    /// Interaction logic for MainV.xaml
    /// </summary>
    public partial class MainV : UserControl, IViewFor<MainVM>, IActivatableView
    {


        public MainV()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // Bind the view model router to RoutedViewHost.Router property.
                this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, x => x.GoNext, x => x.GoNextButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, x => x.GoBack, x => x.GoBackButton)
                    .DisposeWith(disposables);
            });


        }

        public MainVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as MainVM; }
    }
}
