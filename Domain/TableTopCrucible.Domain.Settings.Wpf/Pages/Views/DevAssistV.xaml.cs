using ReactiveUI;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages.Views
{
    /// <summary>
    /// Interaction logic for DevAssistV.xaml
    /// </summary>
    public partial class DevAssistV
    {
        public DevAssistV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.BindCommand(ViewModel, vm => vm.AddTrackerCommand, v => v.AddTracker),
                this.BindCommand(ViewModel, vm => vm.AddNotificationsCommand, v => v.AddNotifications)
            });
        }
    }
}
