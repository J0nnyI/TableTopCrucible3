
using ReactiveUI;

using TableTopCrucible.Domain.Settings.Wpf.PageViewModels;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for MasterDirectorySettingsPv.xaml
    /// </summary>
    public partial class MasterDirectorySettingsCategoryPv : ReactiveUserControl<MasterDirectorySettingsCategoryPvm>, IActivatableView
    {
        public MasterDirectorySettingsCategoryPv()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.Bind(ViewModel,
                    vm=>vm.DirectoryList,
                    v=>v.DirectoryList.ViewModel)
            });
        }
    }
}
