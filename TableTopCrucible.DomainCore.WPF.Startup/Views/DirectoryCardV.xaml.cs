using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;

using TableTopCrucible.DomainCore.WPF.Startup.ViewModels;
using System.Reactive.Linq;
using System.Reactive;
using DirectoryPathVT = TableTopCrucible.Core.ValueTypes.DirectoryPath;
using TableTopCrucible.Core.ValueTypes;
using ReactiveUI.Validation.Extensions;
using TableTopCrucible.Core.WPF.Helper;

namespace TableTopCrucible.DomainCore.WPF.Startup.Views
{
    /// <summary>
    /// Interaction logic for DirectoryCardV.xaml
    /// </summary>
    public partial class DirectoryCardV : ReactiveUserControl<DirectoryCardVM>
    {
        public DirectoryCardV()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.DataContext = this.ViewModel;
                this.Bind(ViewModel, vm => vm.Name, v => v.Description.Text)
                    .DisposeWith(disposables);

                _registerDirectorySelector(PickDirectoryBtn, disposables);
                _registerDirectorySelector(PickThumbnailDirBtn, disposables);

                this.OneWayBind(
                    ViewModel, vm => vm.EditSelector.EditModeEnabled,
                    v => v.PickDirectoryBtn.Visibility,
                    visible => visible ? Visibility.Visible : Visibility.Collapsed);
                this.OneWayBind(
                    ViewModel, vm => vm.EditSelector.EditModeEnabled,
                    v => v.PickThumbnailDirBtn.Visibility,
                    visible => visible ? Visibility.Visible : Visibility.Collapsed);

                this.WhenAnyValue(v=>v.ViewModel.OriginalData)
                    .Subscribe(_ => {
                        ThumbnailDirectory.ScrollToHorizontalEnd();
                        Directory.ScrollToHorizontalEnd();
                    })
                    .DisposeWith(disposables);

                this.ViewModel.UserConfirmationInteraction.RegisterHandler(context =>
                {
                    context.SetOutput(
                        MessageBox.Show(context.Input, "Table Top Crucible", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                            == MessageBoxResult.Yes);
                });
            });
            
        }
        private void _registerDirectorySelector(Button button, CompositeDisposable disposables)
        {
            Observable
                .FromEventPattern(button, nameof(Button.Click))
                .Select(_ => ViewModel.Directory)
                .Select(curPath =>
                {
                    var dialog = new VistaFolderBrowserDialog()
                    {
                        SelectedPath = curPath,
                    };
                    return dialog.ShowDialog() == true
                        ? DirectoryPathVT.From(dialog.SelectedPath)
                        : null;
                })
                .WhereNotNull()
                .Subscribe(newPath=> {
                    ViewModel.UpdateDirectoryPath(newPath);
                    ThumbnailDirectory.ScrollToHorizontalEnd();
                    Directory.ScrollToHorizontalEnd();
                })
                .DisposeWith(disposables);

        }
    }
}
