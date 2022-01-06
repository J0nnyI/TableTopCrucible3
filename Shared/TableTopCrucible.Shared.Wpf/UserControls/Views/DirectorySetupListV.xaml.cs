using System.Reactive.Linq;
using Ookii.Dialogs.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    public partial class DirectorySetupListV : ReactiveUserControl<DirectorySetupListVm>
    {
        public DirectorySetupListV()
        {
            DataContext = ViewModel;

            InitializeComponent();

            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Directories,
                    v => v.DirectoryList.ItemsSource),

                ViewModel!.HintOpacity
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.EmptyListText.Opacity),

                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext),

                this.OneWayBind(ViewModel,
                    vm => vm.CreateDirectory,
                    v => v.CreateDirectory.Command
                ),

                ViewModel!.GetDirectoryDialog.RegisterHandler(interaction =>
                {
                    VistaFolderBrowserDialog dialog = new();
                    interaction.SetOutput(dialog.ShowDialog() == true
                        ? (DirectoryPath)dialog.SelectedPath
                        : null);
                })
            });
        }
    }
}