using ReactiveUI;

using System.Windows.Controls;

using TableTopCrucible.Domain.WPF.Library.PageViewModels;

namespace TableTopCrucible.Domain.WPF.Library.PageViews
{
    /// <summary>
    /// Interaction logic for LibraryPageV.xaml
    /// </summary>
    public partial class LibraryPageV : UserControl, IViewFor<LibraryPageVM>
    {
        public LibraryPageV()
        {
            InitializeComponent();
        }

        public LibraryPageVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as LibraryPageVM; }
    }
}
