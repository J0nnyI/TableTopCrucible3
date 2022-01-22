﻿using System.Linq;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Wpf.Engine
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public static class EngineStarter
    {
        public static void InitializeEngine()
        {
            initializeHost();
            initializeWpf();
            //Application.Current.Startup += 
            //    (_, _) => Locator.Current!.GetService<IMainWindowStarter>()!.Show();
        }

        private static void initializeHost()
        {
            Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    services.AddTtcServices();

                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();
                })
                .ConfigureLogging(loggingBuilder => { loggingBuilder.AddSplat(); })
                .UseEnvironment(
#if DEBUG
                    Environments.Development
#else
            Environments.Production
#endif
                )
                .Build();
        }

        private static void initializeWpf()
        {
            AssemblyHelper
                .SolutionAssemblies.Value
                .ToList()
                .ForEach(Locator.CurrentMutable.RegisterViewsForViewModels);
        }
    }
}