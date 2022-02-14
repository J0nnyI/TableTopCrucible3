using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Win32;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views;

/// <summary>
/// Interaction logic for ItemActionsV.xaml
/// </summary>
public partial class ItemActionsV : ReactiveUserControl<ItemActionsVm>
{
    public ItemActionsV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            this.BindCommand(ViewModel,
                vm => vm.StartSyncCommand,
                v => v.StartSync),
            this.BindCommand(ViewModel,
                vm => vm.DeleteAllDataCommand,
                v => v.DeleteAllData),
            this.BindCommand(ViewModel,
                vm => vm.GenerateThumbnailsCommand,
                v => v.GenerateThumbnails),
            this.BindCommand(ViewModel,
                vm => vm.PickThumbnailsCommand,
                v => v.AddImages),

            this.ViewModel.DeletionConfirmation.RegisterHandler(async interaction =>
            {
                var dialog = Locator.Current
                    .GetService<IDialogService>()
                    .OpenYesNoDialog("Do you really want to delete all your data?" + Environment.NewLine +
                                     "Only your directory setups will remain.");
                interaction.SetOutput(await dialog.Result);
            }),

            this.ViewModel.SelectImages.RegisterHandler(interaction =>
            {
                var extensions = string.Join(";", FileExtension.Image.Select(ex => $"*{ex}"));
                var dialog = new OpenFileDialog()
                {
                    Multiselect = true,
                    Filter = $"Images ({extensions})|{extensions}"
                };
                interaction.SetOutput(
                    dialog.ShowDialog() != true
                        ? Enumerable.Empty<ImageFilePath>()
                        : dialog.FileNames.Select(ImageFilePath.From).ToArray());
            })
        });
    }
}