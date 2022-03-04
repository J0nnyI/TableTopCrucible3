using ReactiveUI;

using System;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages.Views;

/// <summary>
/// Interaction logic for DevAssistV.xaml
/// </summary>
public partial class DevAssistV
{
    public DevAssistV()
    {
        InitializeComponent();
        this.WhenActivated(() => new IDisposable[]
        {
            this.BindCommand(ViewModel, vm => vm.AddTrackerCommand, v => v.AddTracker),
            this.BindCommand(ViewModel, vm => vm.AddNotificationsCommand, v => v.AddNotifications),
            this.Bind(ViewModel, vm => vm.EditChip, v => v.EditChip.ViewModel),
            this.Bind(ViewModel, vm => vm.AddChip, v => v.AddChip.ViewModel),
            this.Bind(ViewModel, vm => vm.Editor, v => v.Editor.ViewModel),

        });
    }
}