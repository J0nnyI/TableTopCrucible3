using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls
{
    [Transient]
    public interface IItemListFilter
    {
    }

    public class ItemListFilterVm : ReactiveObject, IItemListFilter, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();
    }
}