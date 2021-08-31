using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using MaterialDesignColors;

using MaterialDesignThemes.Wpf;

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
    public class EngineApplication : Application
    {
        public EngineApplication()
        {
            initializeWpf();
            initializeHost();
        }

        private void initializeHost()
        {
            Host
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
        }

        private void initializeWpf()
        {
            AssemblyHelper
                .GetSolutionAssemblies()
                .ToList()
                .ForEach(Locator.CurrentMutable.RegisterViewsForViewModels);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Locator.Current!.GetService<IMainWindowStarter>()!.Show();
        }
    }
}
