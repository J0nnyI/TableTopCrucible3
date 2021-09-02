
using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;


namespace TableTopCrucible.Domain.Settings.Wpf.PageViewModels
{
    [Transient(typeof(MasterDirectorySettingsCategoryPvm))]
    public interface IMasterDirectorySettingsCategoryPage : ISettingsCategoryPage
    {

    }
    public class MasterDirectorySettingsCategoryPvm : IActivatableViewModel, IMasterDirectorySettingsCategoryPage
    {
        public IMasterDirectoryList DirectoryList { get; }
        public Name Title => Name.From("File Directories");
        public SortingOrder Position => SortingOrder.From(1);

        public ViewModelActivator Activator { get; } = new();

        public MasterDirectorySettingsCategoryPvm(IMasterDirectoryList directoryList)
        {
            DirectoryList = directoryList;
        }
    }
}
