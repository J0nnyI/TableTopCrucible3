using ReactiveUI;

using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for NotificastionListV.xaml
    /// </summary>
    public partial class NotificationListV : ReactiveUserControl<NotificationListVm>, IActivatableView
    {
        public NotificationListV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.NotificationList,
                            v=>v.Notifications.ItemsSource),
            });
        }
    }
}
