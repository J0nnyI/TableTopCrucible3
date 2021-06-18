using Ookii.Dialogs.Wpf;

using ReactiveUI;

using System.Collections;
using System.Reactive.Disposables;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.DomainCore.WPF.Startup.ViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.Views
{
    /// <summary>
    /// Interaction logic for DirectoryListV.xaml
    /// </summary>
    public partial class DirectoryListV : ReactiveUserControl<DirectoryListVM>
    {
        public DirectoryListV()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.DirectoryCards,
                        v => v.DirectoryCards.ItemsSource,
                        lst => lst as IEnumerable)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.AddDirectory, v => v.addDirectory);

                this.ViewModel.GetDirectoryFromUser.RegisterHandler(
                    interaction =>
                    {
                        var dialog = new VistaFolderBrowserDialog();
                        interaction.SetOutput(
                            dialog.ShowDialog() == true
                            ? DirectoryPath.From(dialog.SelectedPath)
                            : null);

                        this.Bind(ViewModel, vm => vm.Filter, v => v.Filter.Text);

                    });
            });

        }
    }
}
