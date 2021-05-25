using ReactiveUI;

using Splat;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
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
    public class StartupPageVM : ReactiveObject, IStartupPage, IActivatableViewModel
    {
        private readonly ILauncherService _launcherService;

        public StartupPageVM(IRecentMasterFileList masterFileList, ILauncherService launcherService)
        {
            MasterFileList = masterFileList;
            _launcherService = launcherService;
            this.WhenActivated(disposables =>
            {
                OpenDirectoryWizard = ReactiveCommand
                    .Create(() =>
                    {
                        HostScreen.Router.Navigate.Execute(
                            Locator.Current.GetService<IDirectoryWizardPage>());
                    })
                    .DisposeWith(disposables);

                OpenDirectoryWizard.ThrownExceptions.Subscribe(err =>
                {

                });
                HostScreen.Router.Navigate.ThrownExceptions.Subscribe(err =>
                {

                });
            });
        }

        public IRecentMasterFileList MasterFileList { get; }
        public string UrlPathSegment => "SaveFileSelection";
        public IScreen HostScreen => _launcherService.Screen;
        public ReactiveCommand<Unit, Unit> OpenDirectoryWizard { get; private set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
