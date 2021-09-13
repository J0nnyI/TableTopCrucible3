using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Domain.Library.Wpf.Pages.ViewModels
{
    [Singleton(typeof(LibraryPageVm))]
    public interface ILibraryPage : INavigationPage
    {

    }
    public class LibraryPageVm : ReactiveObject, IActivatableViewModel, ILibraryPage
    {
        public ViewModelActivator Activator { get; } = new();
        public PackIconKind? Icon => PackIconKind.Bookshelf;
        public Name Title => Name.From("Item Library");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Upper;
        public SortingOrder Position => SortingOrder.From(1);
    }
}
