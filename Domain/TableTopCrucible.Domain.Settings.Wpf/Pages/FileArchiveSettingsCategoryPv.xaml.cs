
using ReactiveUI;

using TableTopCrucible.Domain.Settings.Wpf.PageViewModels;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages
{
    public partial class FileArchiveSettingsCategoryPv : ReactiveUserControl<FileArchiveNavigationPvm>, IActivatableView
    {
        public FileArchiveSettingsCategoryPv()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.Bind(ViewModel,
                    vm=>vm.FileArchiveList,
                    v=>v.FileArchiveList.ViewModel),
                this.Bind(ViewModel,
                    vm=>vm.Title.Value,
                    v=>v.Title.Text),
            });
        }
    }
}
