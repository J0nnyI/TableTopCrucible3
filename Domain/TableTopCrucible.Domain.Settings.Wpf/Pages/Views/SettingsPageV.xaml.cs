using System.Windows;

using ReactiveUI;

using Splat;

using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Domain.Settings.Wpf.Pages.ViewModels;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages.Views
{
    public partial class SettingsPageV : ReactiveUserControl<SettingsPageVm>
    {
        public SettingsPageV()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var srv = Locator.Current.GetService<IProgressTrackingService>();

            srv.CreateSourceTracker((Name)"ToDo Test");
            srv.CreateSourceTracker((Name)"InProgress Test").Increment((ProgressIncrement).5);
            srv.CreateSourceTracker((Name)"Done Test").Complete();
        }
    }
}
