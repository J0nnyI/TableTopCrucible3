using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.DomainCore.WPF.Startup.Views;

namespace TableTopCrucible.DomainCore.WPF.Startup.ViewModels
{
    [Transient(typeof(RecentMasterFileListVM))]
    public interface IRecentMasterFileList
    {

    }
    public class RecentMasterFileListVM:IRecentMasterFileList
    {
    }
}
