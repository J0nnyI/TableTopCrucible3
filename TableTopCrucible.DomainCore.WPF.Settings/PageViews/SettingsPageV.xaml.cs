using ReactiveUI;

using System.Windows.Controls;

using TableTopCrucible.DomainCore.WPF.Settings.ViewModels;

namespace TableTopCrucible.DomainCore.WPF.Settings.PageViews
{
    /// <summary>
    /// Interaction logic for SettingsPageV.xaml
    /// </summary>
    public partial class SettingsPageV : UserControl, IViewFor<SettingsPageVM>
    {
        public SettingsPageV()
        {
            InitializeComponent();
        }

        public SettingsPageVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as SettingsPageVM; }
    }
}
