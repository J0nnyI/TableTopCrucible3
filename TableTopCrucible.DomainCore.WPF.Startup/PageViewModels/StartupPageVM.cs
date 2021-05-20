using ReactiveUI;

using Splat;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.DomainCore.WPF.Startup.PageViews;
using TableTopCrucible.DomainCore.WPF.Startup.Services;
using TableTopCrucible.DomainCore.WPF.Startup.ViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.PageViewModels
{
    [Singleton(typeof(StartupPageVM))]
    public interface IStartupPage : IRoutableViewModel
    {

    }
    public class StartupPageVM : ReactiveObject, IStartupPage
    {
        private readonly ILauncherService _launcherService;

        public StartupPageVM(IRecentMasterFileList masterFileList, ILauncherService launcherService)
        {
            MasterFileList = masterFileList;
            _launcherService = launcherService;
            
        }

        public IRecentMasterFileList MasterFileList { get; }
        public string UrlPathSegment => "SaveFileSelection";
        public IScreen HostScreen => _launcherService.Screen;
    }
}
