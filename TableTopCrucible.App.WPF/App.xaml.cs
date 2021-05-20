using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ReactiveUI;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

using TableTopCrucible.App.Shared;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.WPF.MainWindow.ViewModels;
using TableTopCrucible.DomainCore.WPF.Startup.PageViewModels;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TableTopCrucible.App.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();


                    DependencyBuilder.GetServices(services);
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSplat();
                })
                .UseEnvironment(Environments.Development)
                .Build();

            AssemblyHelper
                .GetSolutionAssemblies()
                .ToList()
                .ForEach(Locator.CurrentMutable.RegisterViewsForViewModels);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            new Window()
            {
                Title = "TTC Tester",
                Content = new ViewModelViewHost()
                {
                    ViewModel = Locator.Current.GetService<IStartupPage>(),
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                },
                VerticalContentAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            }.Show();



        }
    }
}
