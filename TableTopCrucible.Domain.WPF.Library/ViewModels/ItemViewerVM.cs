
using TableTopCrucible.Core.DI.Attributes;

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
