using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using TableTopCrucible.Core.DependencyInjection;

namespace TableTopCrucible.Core.TestHelper
{
    public static class Prepare
    {
        public static void ApplicationEnvironment(bool includeAutoMapper = false)
        {
            Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();


                    DependencyBuilder.GetServices(services, includeAutoMapper);

                    services.ReplaceFileSystem<MockFileSystem>();
                    services.UseMicrosoftDependencyResolver();
                    Locator.CurrentMutable.InitializeSplat();
                    Locator.CurrentMutable.InitializeReactiveUI();
                })
                .ConfigureLogging(loggingBuilder => { loggingBuilder.AddSplat(); })
                .UseEnvironment(
                    Environments.Development
                )
                .Build();
        }
    }
}