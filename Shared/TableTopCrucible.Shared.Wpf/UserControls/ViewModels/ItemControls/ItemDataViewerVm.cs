using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls
{
    [Transient]
    public interface IItemDataViewer
    {
        Item Item { get; set; }
    }

    public class ItemDataViewerVm : ReactiveObject, IItemDataViewer, IActivatableViewModel
    {
        public ITagEditor TagEditor { get; }
        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public Item Item { get; set; }

        public ItemDataViewerVm(ITagEditor tagEditor)
        {
            TagEditor = tagEditor;
            this.WhenActivated(()=>new []
            {
                this.WhenAnyValue(vm=>vm.Item)
                    .Select(item=>item?.Tags)
                    .BindTo(this, vm=>vm.TagEditor.TagSource)
            });
        }
    }
}