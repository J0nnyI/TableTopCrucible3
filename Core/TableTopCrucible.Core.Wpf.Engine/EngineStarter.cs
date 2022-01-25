using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using TableTopCrucible.Core.DependencyInjection;
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
            initializeJson();
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

        private static void initializeJson()
        {
            var converters =AssemblyHelper
                .GetSolutionClassesOfType<JsonConverter>()
                .Select(t => Activator.CreateInstance(t) as JsonConverter)
                .ToList();
            JsonConvert.DefaultSettings = () => new()
            {
                Converters = converters
            };
        }
    }
}