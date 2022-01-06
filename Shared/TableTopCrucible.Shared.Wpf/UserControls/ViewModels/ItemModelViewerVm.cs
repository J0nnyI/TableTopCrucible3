﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemModelViewer
    {
        public Item Item{get; set; }
    }
    public class ItemModelViewerVm:ReactiveObject,IActivatableViewModel, IItemModelViewer
    {
        public ViewModelActivator Activator { get; } = new();
        [Reactive]
        public Item Item { get; set; }
    }
}
