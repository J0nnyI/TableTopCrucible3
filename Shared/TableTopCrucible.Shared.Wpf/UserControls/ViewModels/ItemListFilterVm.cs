using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemListFilter
    {

    }
    public class ItemListFilterVm:ReactiveObject, IItemListFilter, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();
    }
}
