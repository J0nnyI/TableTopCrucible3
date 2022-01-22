using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemViewerHeader
    {
        public Item Item { get; set; }
    }

    public class ItemViewerHeaderVm : ReactiveObject, IItemViewerHeader, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public Item Item { get; set; }
    }
}