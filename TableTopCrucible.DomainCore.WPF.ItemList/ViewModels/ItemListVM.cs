using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.DomainCore.WPF.ItemList.Views;

using TableTopCrucible.Data.Library.Services.Sources;

namespace TableTopCrucible.DomainCore.WPF.ItemList.ViewModels
{
    [Transient(typeof(ItemListVM))]
    public interface IItemList
    {

    }
    [ViewModel(typeof(ItemListV))]
    class ItemListVM:IItemList
    {
        public ItemListVM(IItemService itemService)
        {

        }
    }
}
