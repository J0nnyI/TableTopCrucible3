
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ReactiveUI;

using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

using System.Linq;
using System.Windows;

using TableTopCrucible.App.Shared;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.DomainCore.WPF.Startup.Services;

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
        }
    }
}
