using System;
using System.Reactive.Linq;
using System.Windows;

using ReactiveUI;

using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    public partial class FileArchiveCardV : ReactiveUserControl<FileArchiveCardVm>, IActivatableView
    {
        public FileArchiveCardV()
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
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.IsDirty,
                    v=>v.SaveChanges.IsEnabled),
                this.WhenAnyValue(
                    v=>v.ViewModel.IsDirty,
                    v=>v.ViewModel.HasErrors,
                    (isDirty, hasErrors)=>isDirty && !hasErrors)
                    .Do(_=>{})
                    .BindTo(this, v=>v.SaveChanges.IsEnabled),
                this.OneWayBind(
                    ViewModel,
                    vm=>vm.IsDirty,
                    v=>v.UndoChanges.IsEnabled),
            });
        }
    }
}
