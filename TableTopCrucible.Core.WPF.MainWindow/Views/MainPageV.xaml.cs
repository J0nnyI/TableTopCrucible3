using ReactiveUI;

using System.Windows.Controls;

using TableTopCrucible.Core.WPF.MainWindow.ViewModels;

namespace TableTopCrucible.Core.WPF.MainWindow.Views
{

    /// <summary>
    /// Interaction logic for MainWindowV.xaml
    /// </summary>
    public partial class MainWindowV : UserControl, IViewFor<MainPageVM>
    {
        public MainWindowV()
        {
            InitializeComponent();
        }

        public MainPageVM ViewModel { get => DataContext as MainPageVM; set => DataContext = value; }
        object IViewFor.ViewModel { get => DataContext; set => DataContext = value; }
    }
}
