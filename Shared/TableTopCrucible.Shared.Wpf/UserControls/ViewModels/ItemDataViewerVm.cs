using System;
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
    public interface IItemDataViewer
    {
        Item Item { get; set; }
    }
    public class ItemDataViewerVm:ReactiveObject, IItemDataViewer, IActivatableViewModel
    {
        [Reactive]
        public Item Item { get; set; }
        public ViewModelActivator Activator { get; } = new();
    }
}
