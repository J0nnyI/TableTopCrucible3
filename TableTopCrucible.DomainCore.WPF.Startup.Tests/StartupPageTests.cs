using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;

using NUnit.Framework;

using ReactiveUI.Testing;

using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

using TableTopCrucible.App.Shared;
using TableTopCrucible.Core.WPF.Testing.Helper;
using TableTopCrucible.Data.Library.DataTransfer.Master;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.DomainCore.WPF.Startup.PageViewModels;
using TableTopCrucible.DomainCore.WPF.Startup.Services;
using TableTopCrucible.DomainCore.WPF.Startup.WindowViewModels;
using TableTopCrucible.DomainCore.WPF.Startup.WindowViews;

namespace TableTopCrucible.DomainCore.WPF.Startup.Tests
{
    [TestFixture]
    [Author("Marcel Rehkemper")]
    public class StartupPageTests
    {
        private IServiceProvider di;
        private StartupPageVM sutVm;
        private IMasterFileService masterFileService;
        private ILauncherService launcherService;
        private ILauncherWindow launcherWindow;
        private LauncherWindow launcherWindowV;
        private CompositeDisposable disposables;

        [SetUp]
        [STAThread]
        public void BeforeEach()
        {
            disposables = new CompositeDisposable();

            di = DependencyBuilder.GetTestProvider(srv => srv.AddSingleton<IFileSystem, MockFileSystem>());
            //ViewSetupHelper.PrepareForTests();
            sutVm = di.GetRequiredService<IStartupPage>() as StartupPageVM;
            masterFileService = di.GetRequiredService<IMasterFileService>();
            launcherService = di.GetRequiredService<ILauncherService>();
            launcherWindow = di.GetRequiredService<ILauncherWindow>();
            //launcherWindowV = launcherService.OpenLauncher() as LauncherWindow;

            //launcherWindowV.Show();
        }
        [TearDown]
        public void AfterEach()
        {
            //launcherWindowV.Close();
            disposables.Dispose();
        }
        private void activateLauncher()
        {
            (launcherWindow as LauncherWindowVM).Activator.Activate();
        }
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Setup_Works()
        {
            new TestScheduler().With(testScheduler =>
            {
                di.Should().NotBeNull();
                sutVm.Should().NotBeNull();
                masterFileService.Should().NotBeNull();
                launcherService.Should().NotBeNull();
                launcherWindow.Should().NotBeNull();
                launcherWindow.Router.CurrentViewModel.Should().NotBeNull();
                launcherService.Screen.Should().NotBeNull();
                launcherService.Screen.Router.CurrentViewModel.Should().NotBeNull();

                bool activated = false;
                {
                    var activator = (launcherWindow as LauncherWindowVM).Activator;

                    Observable.Merge(
                        activator.Activated.Select(_ => true),
                        activator.Deactivated.Select(_ => false)
                    ).StartWith(false)
                    .Subscribe(x => activated = x)
                    .DisposeWith(disposables);
                    activator.Activate();
                }

                testScheduler.AdvanceTo(1000);
                activated.Should().BeTrue();
            });
        }

        [Test]
        public void Creating_a_new_file_works()
        {
            masterFileService.IsOpen.Should().BeFalse();
            launcherService.Screen.Router.CurrentViewModel.Should().BeAssignableTo<IStartupPage>();
            sutVm.OpenDirectoryWizard.Send().Execute();
            launcherService.Screen.Router.CurrentViewModel.Should().BeAssignableTo<IDirectoryWizardPage>();
            masterFileService.IsOpen.Should().BeTrue();
        }
        [Test]
        // Opening a file when there is already a opened one should throw an exception and
        // show one in the notification area
        public void Opening_a_file()
        {
            masterFileService.IsOpen.Should().BeFalse();
            launcherService.Screen.Router.CurrentViewModel.Should().BeAssignableTo<IStartupPage>();
            sutVm.OpenDirectoryWizard.Send().Execute();
            masterFileService.Open(LibraryFilePath.From(@"D:\otherTestfile.ttcl"));
            launcherService.Screen.Router.CurrentViewModel.Should().BeAssignableTo<IDirectoryWizardPage>();
            masterFileService.IsOpen.Should().BeTrue();

        }
    }
}
