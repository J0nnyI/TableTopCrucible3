using ReactiveUI;

using System.Windows.Controls;

using TableTopCrucible.DomainCore.WPF.Startup.ViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.Views
{
    /// <summary>
    /// Interaction logic for RecentMasterFileListV.xaml
    /// </summary>
    public partial class RecentMasterFileListV : UserControl, IViewFor<RecentMasterFileListVM>
    {
        public RecentMasterFileListV()
        {
            InitializeComponent();
        }

        public RecentMasterFileListVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as RecentMasterFileListVM; }
    }
}
