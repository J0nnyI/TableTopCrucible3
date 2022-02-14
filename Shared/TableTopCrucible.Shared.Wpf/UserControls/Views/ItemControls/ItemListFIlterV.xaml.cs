using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views.ItemControls;

public partial class ItemListFilterV : ReactiveUserControl<ItemListFilterVm>
{
    public ItemListFilterV()
    {
        InitializeComponent();
        this.WhenActivated(() => new[]
        {
            this.Bind(ViewModel,
                vm => vm.IncludeFilter,
                v => v.IncludeFilter.ViewModel),
            this.Bind(ViewModel,
                vm => vm.ExcludeFilter,
                v => v.ExcludeFilter.ViewModel),
        });
    }
}