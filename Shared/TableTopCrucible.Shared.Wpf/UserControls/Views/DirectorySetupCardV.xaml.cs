﻿using System;
using System.Reactive.Linq;
using System.Windows;

using ReactiveUI;

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
                            var res = await MaterialDesignThemes.Wpf.DialogHost.Show(
                                $"This will neither delete your local files not will you loose the data you attached to the files in this directory." +
                                Environment.NewLine +
                                $"You can link the data again by adding a directory with the same files.");
       
                            interaction.SetOutput(true);
                        });
                    })
            });
        }
    }
}