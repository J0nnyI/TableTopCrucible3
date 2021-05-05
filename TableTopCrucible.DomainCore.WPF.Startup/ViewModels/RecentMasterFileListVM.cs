using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.DomainCore.WPF.Startup.Views;

namespace TableTopCrucible.DomainCore.WPF.Startup.ViewModels
{
    [Transient(typeof(RecentMasterFileListVM))]
    public interface IRecentMasterFileList
    {

    }
    [ViewModel(typeof(RecentMasterFileListV))]
    public class RecentMasterFileListVM:IRecentMasterFileList
    {
    }
}
