using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages.ViewModels
{
    [Singleton]
    public interface IDirectorySetupPage : INavigationPage
    {

    }
    public class DirectorySetupPageVm : IActivatableViewModel, IDirectorySetupPage
    {
        public IDirectorySetupList DirectorySetupList { get; }
        public IDirectorySetupCard NewDirectoryCard { get; }
        public PackIconKind? Icon => PackIconKind.FolderSearch;
        public Name Title => Name.From("Directory Setup");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Lower;
        public SortingOrder Position => SortingOrder.From(1);

        public ViewModelActivator Activator { get; } = new();

        public DirectorySetupPageVm(
            IDirectorySetupList directorySetupList,
            IDirectorySetupCard newDirectoryCard)
        {
            DirectorySetupList = directorySetupList;
            NewDirectoryCard = newDirectoryCard;
            NewDirectoryCard.ResetOnSave = true;
        }
    }
}
