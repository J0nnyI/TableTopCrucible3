using ReactiveUI;

using TableTopCrucible.Domain.Settings.Wpf.PageViewModels;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for ApplicationBehavior.xaml
    /// </summary>
    public partial class ApplicationBehaviorPv : ReactiveUserControl<ApplicationBehaviorPvm>
    {
        public ApplicationBehaviorPv()
        {
            InitializeComponent();
        }
    }
}
