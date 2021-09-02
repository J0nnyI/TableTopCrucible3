using ReactiveUI;

using System;

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
