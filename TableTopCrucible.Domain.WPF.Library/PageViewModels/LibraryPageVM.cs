
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Domain.WPF.Library.ViewModels;
using TableTopCrucible.DomainCore.WPF.ItemList.ViewModels;

namespace TableTopCrucible.Domain.WPF.Library.PageViewModels
{
    [Singleton(typeof(LibraryPageVM))]
    public interface ILibraryPage
    {

    }
    public class LibraryPageVM : ILibraryPage
    {
        public LibraryPageVM(IItemList itemList, IItemViewer itemViewer)
        {
            ItemList = itemList;
            ItemViewer = itemViewer;
        }

        public IItemList ItemList { get; }
        public IItemViewer ItemViewer { get; }
    }
}
