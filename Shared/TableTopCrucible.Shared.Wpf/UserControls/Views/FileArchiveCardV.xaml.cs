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
            });
        }
    }
}
