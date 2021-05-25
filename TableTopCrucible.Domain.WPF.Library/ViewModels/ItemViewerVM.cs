
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Domain.WPF.Library.Views;

namespace TableTopCrucible.Domain.WPF.Library.ViewModels
{
    [Transient(typeof(ItemViewerVM))]
    public interface IItemViewer
    {

    }
    public class ItemViewerVM : IItemViewer
    {
    }
}
