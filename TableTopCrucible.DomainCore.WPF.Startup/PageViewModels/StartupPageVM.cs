using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.DomainCore.WPF.Startup.PageViews;
using TableTopCrucible.DomainCore.WPF.Startup.ViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.PageViewModels
{
    [Singleton(typeof(StartupPageVM))]
    public interface IStartupPage
    {

    }
    [ViewModel(typeof(StartupPageV))]
    public class StartupPageVM:IStartupPage
    {
        public StartupPageVM(IRecentMasterFileList masterFileList)
        {
            MasterFileList = masterFileList;
        }

        public IRecentMasterFileList MasterFileList { get; }
    }
}
