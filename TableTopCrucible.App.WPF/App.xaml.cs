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
using TableTopCrucible.DomainCore.WPF.Startup.Services;
using TableTopCrucible.DomainCore.WPF.Startup.WindowViews;

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

            // https://stackoverflow.com/questions/431940/how-to-set-default-wpf-window-style-in-app-xaml
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
            {
                DefaultValue = Application.Current.FindResource(typeof(Window))
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Locator.Current.GetService<ILauncherService>().OpenLauncher();
            //new Window()
            //{
            //    Title = "TTC Tester",
            //    Content = new ViewModelViewHost()
            //    {
            //        ViewModel = Locator.Current.GetService<IStartupPage>(),
            //        VerticalContentAlignment = VerticalAlignment.Stretch,
            //        HorizontalContentAlignment = HorizontalAlignment.Stretch
            //    },
            //    VerticalContentAlignment = VerticalAlignment.Stretch,
            //    HorizontalContentAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //}.Show();



        }
    }
}
