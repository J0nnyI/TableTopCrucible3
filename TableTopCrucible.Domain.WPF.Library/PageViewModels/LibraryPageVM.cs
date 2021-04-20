
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.Domain.WPF.Library.PageViews;
using TableTopCrucible.Domain.WPF.Library.ViewModels;
using TableTopCrucible.DomainCore.WPF.ItemList.ViewModels;

namespace TableTopCrucible.Domain.WPF.Library.PageViewModels
{
    [Singleton(typeof(LibraryPageVM))]
    public interface ILibraryPage
    {

    }
    [ViewModel(typeof(LibraryPageV))]
    internal class LibraryPageVM : PageViewModelBase, ILibraryPage
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
