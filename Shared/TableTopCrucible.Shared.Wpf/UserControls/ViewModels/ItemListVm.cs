﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient(typeof(ItemListVm))]
    public interface IItemList
    {

    }
    public class ItemListVm:ReactiveObject, IItemList, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();
    }
}
