using ReactiveUI;

using System.Windows.Controls;

using TableTopCrucible.Core.Jobs.WPF.ViewModels;

namespace TableTopCrucible.Core.Jobs.WPF.Views
{
    /// <summary>
    /// Interaction logic for JobOverviewV.xaml
    /// </summary>
    public partial class JobOverviewV : UserControl, IViewFor<JobOverviewVM>
    {
        public JobOverviewV()
        {
            InitializeComponent();
        }
        public JobOverviewVM ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as JobOverviewVM;
        }
    }
}
