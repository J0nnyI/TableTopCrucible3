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
                this.Bind(ViewModel, vm => vm.Description, v => v.Description.Text)
                    .DisposeWith(disposables);
                //this.BindValidation(ViewModel, vm=>vm.Description, v=>v.Description.err)
                //this.Bind(ViewModel, vm => vm.DirectoryPath, v => v.DirectoryPath.Text)
                //    .DisposeWith(disposables);

                Observable
                    .FromEventPattern(PickDirectoryBtn, nameof(PickDirectoryBtn.Click))
                    .Select(_ => ViewModel.DirectoryPath)
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
                    .Where(newPath => newPath != null)
                    .Subscribe(ViewModel.UpdateDirectoryPath)
                    .DisposeWith(disposables);

            });
        }
    }
}
