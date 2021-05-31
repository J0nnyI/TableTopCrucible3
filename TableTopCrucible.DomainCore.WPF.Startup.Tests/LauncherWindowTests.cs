using TableTopCrucible.DomainCore.WPF.Startup.WindowViews;
using System;
using System.Collections.Generic;
using System.Text;
using TableTopCrucible.App.Shared;
using ReactiveUI;
using Microsoft.Reactive.Testing;
using Microsoft.Extensions.DependencyInjection;
using TableTopCrucible.DomainCore.WPF.Startup.WindowViewModels;
using NUnit.Framework;
using FluentAssertions;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace TableTopCrucible.DomainCore.WPF.Startup.WindowViews.Tests
{
    public class LauncherWindowTests
    {
        public static void GetEnvironment(out IServiceProvider di, out ILauncherWindow sutVM, out LauncherWindow sutV)
        {
            di = DependencyBuilder.BuildDependencyProvider();
            RxApp.MainThreadScheduler = RxApp.TaskpoolScheduler = new TestScheduler();
            sutVM = di.GetRequiredService<ILauncherWindow>();
            sutV = di.GetRequiredService<IViewFor<LauncherWindowVM>>() as LauncherWindow;
            sutV.ViewModel = sutVM as LauncherWindowVM;

        }

        [SetUp]
        public void beforeEach()
        {

        }
        [Apartment(ApartmentState.STA)]
        [TearDown]
        public void afterEach()
        {
        }
        [Test]
        public void TestButton()
        {

            var btn = new Button();
            btn.Click += (s, e) =>
            {

            };
            ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
            IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }
        [Test]
        public void EnvironmentWorks()
        {
            GetEnvironment(out var di, out var sutVM, out var sutV);
            sutVM.Should().NotBeNull();
            sutV.Should().NotBeNull();
            sutV.ViewModel.Should().BeSameAs(sutVM);
        }
    }
}