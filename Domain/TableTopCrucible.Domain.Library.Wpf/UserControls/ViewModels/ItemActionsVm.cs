using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemActions
    {

    }
    public class ItemActionsVm:ReactiveObject, IItemActions, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();

    }
}
