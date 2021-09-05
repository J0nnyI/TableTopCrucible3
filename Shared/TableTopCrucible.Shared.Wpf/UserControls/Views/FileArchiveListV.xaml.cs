﻿using Ookii.Dialogs.Wpf;

using ReactiveUI;

using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

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

                ViewModel!.GetDirectoryDialog.RegisterHandler(async interaction =>
                {
                    VistaFolderBrowserDialog dialog = new();
                    interaction.SetOutput(dialog.ShowDialog() == true 
                        ? FileArchivePath.From(dialog.SelectedPath) 
                        : null);
                })
            });
        }

    }
}
