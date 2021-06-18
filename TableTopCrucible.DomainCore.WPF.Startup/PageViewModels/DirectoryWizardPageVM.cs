using ReactiveUI;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.DomainCore.WPF.Startup.Services;
using TableTopCrucible.DomainCore.WPF.Startup.ViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.PageViewModels
{
    [Transient(typeof(DirectoryWizardPageVM))]
    public interface IDirectoryWizardPage : IRoutableViewModel
    {

    }
    public class DirectoryWizardPageVM : ReactiveObject, IDirectoryWizardPage,IActivatableViewModel
    {
        private readonly ILauncherService _launcherService;

        public DirectoryWizardPageVM(ILauncherService launcherService, IDirectoryList directoryList)
        {
            _launcherService = launcherService;
            DirectoryList = directoryList;
            //if (VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            //{
            //    new VistaFolderBrowserDialog().ShowDialog();
            //}
        }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public string UrlPathSegment => "DirectoryWizard";
        public IScreen HostScreen => _launcherService.Screen;

        public IDirectoryList DirectoryList { get; }
    }
}
