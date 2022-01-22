using System;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    public partial class DirectorySetupCardV : ReactiveUserControl<DirectorySetupCardVm>, IActivatableView
    {
        public DirectorySetupCardV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext),
                this.Bind(
                    ViewModel,
                    vm => vm.SaveChangesCommand,
                    v => v.SaveChanges.Command),
                this.Bind(
                    ViewModel,
                    vm => vm.UndoChangesCommand,
                    v => v.UndoChanges.Command),
                this.Bind(
                    ViewModel,
                    vm => vm.RemoveDirectoryCommand,
                    v => v.RemoveDirectory.Command),
                this.Bind(
                    ViewModel,
                    vm => vm.Path,
                    v => v.DirectoryPicker.UserText),
                DirectoryPicker
                    .DialogConfirmed
                    .Select(dir => dir.GetDirectoryName().ToName())
                    .BindTo(this,
                        v => v.ViewModel.Name),
                this.Bind(
                    ViewModel,
                    vm => vm.RemoveDirectoryCommand,
                    v => v.RemoveDirectory.Command),
                this.Bind(
                    ViewModel,
                    vm => vm.UndoChangesCommand,
                    v => v.UndoChanges.Command),
                this.Bind(
                    ViewModel,
                    vm => vm.SaveChangesCommand,
                    v => v.SaveChanges.Command),

                ObservableExtensions.Subscribe(this.WhenAnyValue(
                    v => v.ViewModel), vm =>
                {
                    vm.ConfirmDeletionInteraction.RegisterHandler(async interaction =>
                    {
                        var dialog = Locator.Current
                            .GetService<IDialogService>()
                            .OpenYesNoDialog("Do you really want to remove this directory?" + Environment.NewLine +
                                             "No data will be lost.");
                        interaction.SetOutput(await dialog.Result);
                    });
                })
            });
        }
    }
}