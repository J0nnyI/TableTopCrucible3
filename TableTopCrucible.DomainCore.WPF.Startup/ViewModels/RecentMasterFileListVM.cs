
using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.DomainCore.WPF.Startup.ViewModels
{
    [Transient(typeof(RecentMasterFileListVM))]
    public interface IRecentMasterFileList
    {

    }
    public class RecentMasterFileListVM : IRecentMasterFileList
    {
    }
}
