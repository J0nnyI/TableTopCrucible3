
using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;


namespace TableTopCrucible.Domain.Settings.Wpf.PageViewModels
{
    [Transient(typeof(FileArchiveSettingsCategoryPvm))]
    public interface IFileArchiveSettingsCategoryPage : ISettingsCategoryPage
    {

    }
    public class FileArchiveSettingsCategoryPvm : IActivatableViewModel, IFileArchiveSettingsCategoryPage
    {
        public IFileArchiveList FileArchiveList { get; }
        public IFileArchiveCard NewDirectoryCard { get; }
        public Name Title => Name.From("File Archives");
        public SortingOrder Position => SortingOrder.From(1);

        public ViewModelActivator Activator { get; } = new();

        public FileArchiveSettingsCategoryPvm(
            IFileArchiveList fileArchiveList,
            IFileArchiveCard newDirectoryCard)
        {
            FileArchiveList = fileArchiveList;
            NewDirectoryCard = newDirectoryCard;
            NewDirectoryCard.ResetOnSave = true;
        }
    }
}
