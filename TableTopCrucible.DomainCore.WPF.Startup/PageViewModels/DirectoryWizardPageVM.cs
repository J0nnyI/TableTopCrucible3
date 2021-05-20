using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.DomainCore.WPF.Startup.Services;

namespace TableTopCrucible.DomainCore.WPF.Startup.PageViewModels
{
    [Transient(typeof(DirectoryWizardPageVM))]
    public interface IDirectoryWizardPage : IRoutableViewModel
    {

    }
    public class DirectoryWizardPageVM : ReactiveObject, IDirectoryWizardPage,IActivatableViewModel
    {
        private readonly ILauncherService _launcherService;

        public DirectoryWizardPageVM(ILauncherService launcherService)
        {
            _launcherService = launcherService;
        }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public string UrlPathSegment => "DirectoryWizard";
        public IScreen HostScreen => _launcherService.Screen;
    }
}
