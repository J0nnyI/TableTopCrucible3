using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using TableTopCrucible.Core.DependencyInjection;

namespace TableTopCrucible.Core.TestHelper
{
    // test environment setups
    public static class Prepare
    {
        // prepares an integration application environment
        public static void ApplicationEnvironment()
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

                    services.ReplaceFileSystem<MockFileSystem>();
                })
                .ConfigureLogging(loggingBuilder => { loggingBuilder.AddSplat(); })
                .UseEnvironment(
                    Environments.Development
                )
                .Build();
        }
    }
}