using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views
{
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

                this.ViewModel.DeletionConfirmation.RegisterHandler(async interaction =>
                {
                    var dialog = Locator.Current
                        .GetService<IDialogService>()
                        .OpenYesNoDialog("Do you really want to delete all your data?" + Environment.NewLine +
                                         "Only your directory setups will remain.");
                    interaction.SetOutput(await dialog.Result);
                })
            });
        }
    }
}
