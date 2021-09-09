
using MaterialDesignThemes.Wpf;
using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;


namespace TableTopCrucible.Domain.Settings.Wpf.PageViewModels
{
    [Transient(typeof(FileArchiveNavigationPvm))]
    public interface IFileArchiveNavigationPage : INavigationPage
    {

    }
    public class FileArchiveNavigationPvm : IActivatableViewModel, IFileArchiveNavigationPage
    {
        public IFileArchiveList FileArchiveList { get; }
        public IFileArchiveCard NewDirectoryCard { get; }
        public PackIconKind? Icon => PackIconKind.FolderSearch;
        public Name Title => Name.From("Directory Configuration");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Lower;
        public SortingOrder Position => SortingOrder.From(1);

        public ViewModelActivator Activator { get; } = new();

        public FileArchiveNavigationPvm(
            IFileArchiveList fileArchiveList,
            IFileArchiveCard newDirectoryCard)
        {
            FileArchiveList = fileArchiveList;
            NewDirectoryCard = newDirectoryCard;
            NewDirectoryCard.ResetOnSave = true;
        }
    }
}
