using System;
using System.Windows;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views
{
    /// <summary>
    ///     Interaction logic for SimpleNotification.xaml
    /// </summary>
    public partial class SimpleNotification : ReactiveUserControl<SimpleNotificationVm>
    {
        public SimpleNotification()
        {
            InitializeComponent();

            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext),
                this.Bind(
                    ViewModel,
                    vm => vm.Title.Value,
                    v => v.Title.Text),
                this.Bind(
                    ViewModel,
                    vm => vm.Content.Value,
                    v => v.Content.Text),
                this.WhenAnyValue(
                        v => v.ViewModel.Content,
                        c => string.IsNullOrWhiteSpace(c?.Value)
                            ? Visibility.Collapsed
                            : Visibility.Visible)
                    .BindTo(this, v => v.Content.Visibility),
                this.OneWayBind(
                    ViewModel,
                    vm => vm.CloseNotificationCommand,
                    v => v.CloseNotification.Command),
                this.OneWayBind(ViewModel,
                    vm => vm.CardOpacity,
                    v => v.Opacity),
                this.OneWayBind(
                    ViewModel,
                    vm => vm.Type,
                    v => v.Icon.Kind,
                    type => type switch
                    {
                        NotificationType.Info => PackIconKind.InfoCircle,
                        NotificationType.Confirmation => PackIconKind.CheckCircle,
                        NotificationType.Error => PackIconKind.Error,
                        NotificationType.Warning => PackIconKind.WarningCircle,
                        _ => throw new NotImplementedException(nameof(type) + " has no icon")
                    })
            });
        }
    }
}