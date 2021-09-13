using ReactiveUI;

using TableTopCrucible.Domain.Settings.Wpf.Pages.ViewModels;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages.Views
{
    public partial class DirectorySetupPageV : ReactiveUserControl<DirectorySetupPageVm>, IActivatableView
    {
        public DirectorySetupPageV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.Bind(ViewModel,
                    vm=>vm.DirectorySetupList,
                    v=>v.DirectorySetupList.ViewModel),
            });
        }
    }
}
