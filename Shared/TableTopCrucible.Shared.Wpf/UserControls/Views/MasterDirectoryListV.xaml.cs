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
    /// <summary>
    /// Interaction logic for MasterDirectoryListV.xaml
    /// </summary>
    public partial class MasterDirectoryListV : ReactiveUserControl<MasterDirectoryListVm>
    {
        public MasterDirectoryListV()
        {
            this.DataContext = ViewModel;

            InitializeComponent();

            this.WhenActivated(() => new []
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
            });
        }

        private void _SelectDirectoryClicked(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new();
            if (dialog.ShowDialog() == true)
            {
                ViewModel.Directory = dialog.SelectedPath;
                ViewModel.Name = DirectoryPath.From(ViewModel.Directory).GetDirectoryName().Value;
            }
        }
    }
}
