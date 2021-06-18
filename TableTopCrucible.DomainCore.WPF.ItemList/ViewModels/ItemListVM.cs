
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.Services.Sources;

namespace TableTopCrucible.DomainCore.WPF.ItemList.ViewModels
{
    [Transient(typeof(ItemListVM))]
    public interface IItemList
    {

    }
    class ItemListVM : IItemList
    {
        public ItemListVM(IItemService itemService)
        {

        }
    }
}
