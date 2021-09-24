
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.DependencyInjection;
using TableTopCrucible.Core.Wpf.Engine;

namespace TableTopCrucible.Core.TestHelper
{
    public static class Prepare
    {
        public static void ApplicationEnvironment(bool includeAutomapper = false)
        {
            Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();


                    DependencyBuilder.GetServices(services, includeAutomapper);

                    services.ReplaceFileSystem<MockFileSystem>();
                    services.UseMicrosoftDependencyResolver();
                    Locator.CurrentMutable.InitializeSplat();
                    Locator.CurrentMutable.InitializeReactiveUI();

                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSplat();
                })
                .UseEnvironment(
                    Environments.Development
                )
                .Build();
        }
    }
}
