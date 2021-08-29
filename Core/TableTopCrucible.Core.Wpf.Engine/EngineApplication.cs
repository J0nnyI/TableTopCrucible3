using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Microsoft.Extensions.Hosting;

using ReactiveUI;

using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Views.Windows;
using TableTopCtucible.Core.DependencyInjection;

namespace TableTopCrucible.Core.Wpf.Engine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class EngineApplication: Application
    {
        public EngineApplication()
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
                .UseEnvironment(
#if DEBUG
                    Environments.Development
#else
                Environments.Production
#endif
                )
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
            Locator.Current.GetService<IMainWindowStarter>().Show();
        }
    }
}
