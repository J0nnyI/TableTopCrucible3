using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemViewerHeader
    {
    }

    public class ItemViewerHeaderVm : ReactiveObject, IItemViewerHeader, IActivatableViewModel
    {
        public IIconTabStrip TabStrip { get; }
        public ViewModelActivator Activator { get; } = new();

        private ObservableAsPropertyHelper<Item> _item;
        public Item Item => _item.Value;

        public ItemViewerHeaderVm(ILibraryService libraryService, IIconTabStrip tabStrip)
        {
            TabStrip = tabStrip;
            TabStrip.Init(libraryService);
            _item = libraryService.SingleSelectedItemChanges.ToProperty(this, vm => vm.Item);
        }
    }
}