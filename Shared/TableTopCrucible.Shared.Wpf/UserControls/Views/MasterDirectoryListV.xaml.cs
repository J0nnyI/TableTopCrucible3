using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    /// Interaction logic for MasterDirectoryListV.xaml
    /// </summary>
    public partial class MasterDirectoryListV : ReactiveUserControl<MasterDirectoryListVm>
    {
        public MasterDirectoryListV()
        {
            InitializeComponent();
            this.WhenActivated(() => new IDisposable[]
            {
                this.OneWayBind(ViewModel,
                    vm=>vm.Directories,
                    v=>v.DirectoryList.ItemsSource),

                this.OneWayBind(ViewModel,
                    vm=>vm.Directory,
                    v=>v.Directory.Text),

                this.OneWayBind(ViewModel,
                    vm=>vm.Name,
                    v=>v.Name.Text),

                this.OneWayBind(ViewModel, 
                    vm=>vm.CreateDirectory,
                    v=>v.CreateDirectory.Command
                    )
            });
        }
    }
}
