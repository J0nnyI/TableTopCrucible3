using ReactiveUI;

using System;
using System.Collections;
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
                this.Bind(ViewModel, vm => vm.TemporaryDirectoryCard, v => v.TemporaryDirectoryCard.ViewModel)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, 
                        vm => vm.DirectoryCards,
                        v => v.DirectoryCards.ItemsSource,
                        lst=>lst as IEnumerable)
                    .DisposeWith(disposables);
            });
        }
    }
}
