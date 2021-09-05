using System.Windows;
using System.Windows.Controls;

using ReactiveUI;

using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    /// Interaction logic for SimpleNotification.xaml
    /// </summary>
    public partial class SimpleNotification : ReactiveUserControl<SimpleNotificationVm>
    {
        public SimpleNotification()
        {
            InitializeComponent();

            this.WhenActivated(() => new[]
            {
                this.Bind(
                    ViewModel,
                    vm=>vm.Title.Value,
                    v=>v.Title.Text),
                this.Bind(
                    ViewModel,
                    vm=>vm.Content.Value,
                    v=>v.Content.Text),
                this.WhenAnyValue(
                    v=>v.ViewModel)
                    .BindTo(this, v=>v.DataContext),
                this.WhenAnyValue(
                    v=>v.ViewModel.Content,
                    c=>string.IsNullOrWhiteSpace(c.Value)
                        ?Visibility.Collapsed
                        :Visibility.Visible)
                    .BindTo(this, v=>v.Content.Visibility),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.DeleteCountdownTotal,
                    v=>v.DeleteCountdown.Maximum),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.DeleteCountdownProgress,
                    v=>v.DeleteCountdown.Value),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.DeleteCountdownRunning,
                    v=>v.DeleteCountdown.Visibility,
                    running=>running?Visibility.Visible:Visibility.Collapsed),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.CloseNotificationCommand,
                    v=>v.CloseNotification.Command)
            });
        }
    }
}
