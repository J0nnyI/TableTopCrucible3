using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views.ItemControls
{
    public partial class ItemListFilterV : ReactiveUserControl<ItemListFilterVm>
    {
        public ItemListFilterV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.Bind(ViewModel,
                    vm => vm.IncludeTags,
                    v => v.IncludeTags.ViewModel),
                this.Bind(ViewModel,
                    vm => vm.ExcludeTags,
                    v => v.ExcludeTags.ViewModel)
            });
        }
    }
}