using System;
using System.Windows;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.Views;

/// <summary>
///     Interaction logic for SimpleNotification.xaml
/// </summary>
public partial class NotificationInfoV : ReactiveUserControl<NotificationInfoVm>
{
    public NotificationInfoV()
    {
        InitializeComponent();
        CloseNotification.SetValue(ButtonProgressAssist.IsIndicatorVisibleProperty, true);
        this.WhenActivated(() => new[]
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext),
            this.Bind(
                ViewModel,
                vm => vm.Notification.Title.Value,
                v => v.Title.Text),
            this.Bind(
                ViewModel,
                vm => vm.Notification.Content.Value,
                v => v.Content.Text),
            this.Bind(ViewModel,
                vm => vm.Notification.IsExpanded,
                v => v.Expander.IsExpanded),
            //this.OneWayBind(ViewModel,
            //    vm=>vm.IsExpanded,
            //    v=>v.ExpanderIcon.Kind,
            //    expanded=>expanded?PackIconKind.ExpandLess:PackIconKind.ExpandMore),
            //this.OneWayBind(ViewModel,
            //    vm => vm.CardOpacity,
            //    v => v.Opacity),
            this.OneWayBind(
                ViewModel,
                vm => vm.Notification.Type,
                v => v.Icon.Kind,
                type => type switch
                {
                    NotificationType.Info => PackIconKind.InfoCircle,
                    NotificationType.Confirmation => PackIconKind.CheckCircle,
                    NotificationType.Error => PackIconKind.Error,
                    NotificationType.Warning => PackIconKind.WarningCircle,
                    _ => throw new NotImplementedException(nameof(type) + " has no icon")
                }),

            this.BindCommand(ViewModel,
                vm => vm.CloseNotificationCommand,
                v => v.CloseNotification),
            this.OneWayBind(ViewModel,
                vm => vm.ProvideClose,
                v => v.CloseNotification.Visibility,
                show => show
                    ? Visibility.Visible
                    : Visibility.Collapsed),

            this.OneWayBind(ViewModel,
                vm => vm.TicksTotal,
                v => v.CountDownTimer.Maximum),
            this.OneWayBind(ViewModel,
                vm => vm.TicksRemaining,
                v => v.CountDownTimer.Value),
            this.OneWayBind(ViewModel,
                vm => vm.ShowTimer,
                v => v.CountDownTimer.Visibility,
                visible => visible
                    ? Visibility.Visible
                    : Visibility.Collapsed),
        });
    }
}