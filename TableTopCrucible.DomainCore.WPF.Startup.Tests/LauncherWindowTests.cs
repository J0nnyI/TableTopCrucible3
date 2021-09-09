using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;

using NUnit.Framework;

using ReactiveUI;

using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive;
using System.Threading;
using System.Windows.Controls;

using TableTopCrucible.App.Shared;
using TableTopCrucible.Core.WPF.Testing.Helper;
using TableTopCrucible.DomainCore.WPF.Startup.PageViewModels;
using TableTopCrucible.DomainCore.WPF.Startup.WindowViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.WindowViews.Tests
{
    [TestFixture]
    public class LauncherWindowTests
    {
        private IStartupPage startupPage;
        private IServiceProvider di;
        private ILauncherWindow sutVM;
        private LauncherWindow sutV;

        [SetUp]
        public void BeforeEach()
        {
            di = DependencyBuilder.GetTestProvider(srv => srv.AddSingleton<IFileSystem, MockFileSystem>());
            RxApp.MainThreadScheduler = RxApp.TaskpoolScheduler = new TestScheduler();
            //sutVM = di.GetRequiredService<ILauncherWindow>();
            sutV = di.GetRequiredService<IViewFor<LauncherWindowVM>>() as LauncherWindow;
            startupPage = di.GetRequiredService<IStartupPage>();
            sutV.ViewModel = sutVM as LauncherWindowVM;

        }
        [Apartment(ApartmentState.STA)]
        [Test]
        public void TestButton()
        {
            var a = 0;
            var btn = new Button();
            btn.Click += (s, e) => a = 1;
            btn.Send().Click();
            a.Should().Be(0);
        }
        [Apartment(ApartmentState.STA)]
        [Test]
        public void testButtonCommand()
        {
            var a = 0;

            var btn = new Button();
            btn.Command = ReactiveCommand.Create<Unit>(_ => a = 1);
            btn.Command.Execute(null);
            a.Should().Be(1);
        }
        [Test]
        public void EnvironmentWorks()
        {
            sutVM.Should().NotBeNull();
            sutV.Should().NotBeNull();
            startupPage.Should().NotBeNull();
            sutV.ViewModel.Should().BeSameAs(sutVM);
        }
    }
}