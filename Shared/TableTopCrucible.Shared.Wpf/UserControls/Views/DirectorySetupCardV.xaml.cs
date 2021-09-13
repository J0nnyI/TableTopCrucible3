using ReactiveUI;

using Splat;

using System;
using System.Reactive.Linq;

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
                this.WhenAnyValue(v=>v.ViewModel)
                    .BindTo(this, v=>v.DataContext),
                this.Bind(
                    ViewModel,
                    vm=>vm.SaveChangesCommand,
                    v=>v.SaveChanges.Command),
                this.Bind(
                    ViewModel,
                    vm=>vm.UndoChangesCommand,
                    v=>v.UndoChanges.Command),
                this.Bind(
                    ViewModel,
                    vm=>vm.RemoveDirectoryCommand,
                    v=>v.RemoveDirectory.Command),
                this.Bind(
                    ViewModel,
                    vm=>vm.Path,
                    v=>v.DirectoryPicker.UserText),
                this.Bind(
                    ViewModel,
                    vm=>vm.RemoveDirectoryCommand,
                    v=>v.RemoveDirectory.Command),
                this.Bind(
                    ViewModel,
                    vm=>vm.UndoChangesCommand,
                    v=>v.UndoChanges.Command),
                this.Bind(
                    ViewModel,
                    vm=>vm.SaveChangesCommand,
                    v=>v.SaveChanges.Command),

                this.WhenAnyValue(
                    v=>v.ViewModel)
                    .Subscribe(vm =>
                    {
                        vm.ConfirmDeletionInteraction.RegisterHandler(async interaction =>
                        {
                            var dialog = Locator.Current
                                .GetService<IDialogService>()
                                .OpenYesNoDialog("Do you really want to remove this directory?" +Environment.NewLine +
                                                          "No Data will be lost.");
                            interaction.SetOutput(await dialog.Result);
                        });
                    })
            });
        }
    }
}
