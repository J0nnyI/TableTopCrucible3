using ReactiveUI;

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;
using Ookii.Dialogs.Wpf;
using ReactiveUI.Validation.Extensions;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;
using EventExtensions = System.Windows.EventExtensions;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    public partial class FileArchiveListV : ReactiveUserControl<FileArchiveListVm>
    {
        public FileArchiveListV()
        {
            this.DataContext = ViewModel;

            InitializeComponent();

            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm=>vm.Directories,
                    v=>v.DirectoryList.ItemsSource),

                this.WhenAnyValue(v=>v.ViewModel)
                    .BindTo(this, v=>v.DataContext),

                this.OneWayBind(ViewModel,
                    vm=>vm.CreateDirectory,
                    v=>v.CreateDirectory.Command
                    ),

                this.OneWayBind(ViewModel,
                    vm=>vm.CreateDirectory,
                    v=>v.CreateDirectory.Command),

                ViewModel!.GetDirectoryDialog.RegisterHandler(async interaction =>
                {
                    VistaFolderBrowserDialog dialog = new();
                    if( dialog.ShowDialog() == true)
                       interaction.SetOutput( FileArchivePath.From(dialog.SelectedPath));
                    else
                       interaction.SetOutput( null);
                })
            });
        }

    }
}
