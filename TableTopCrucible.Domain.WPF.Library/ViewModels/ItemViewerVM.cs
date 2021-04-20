
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.Domain.WPF.Library.Views;

namespace TableTopCrucible.Domain.WPF.Library.ViewModels
{
    [Transient(typeof(ItemViewerVM))]
    public interface IItemViewer
    {

    }
    [ViewModel(typeof(ItemViewerV))]
    class ItemViewerVM : IItemViewer
    {
    }
}
