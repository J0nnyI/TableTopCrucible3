using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using TableTopCrucible.Core.DependencyInjection;

namespace TableTopCrucible.Core.TestHelper;

// test environment setups
public static class Prepare
{
    // prepares an integration application environment
    public static IHost ApplicationEnvironment()
    {
        return Host
            .CreateDefaultBuilder()
            .ConfigureServices(Services)
            .ConfigureLogging(loggingBuilder => { loggingBuilder.AddSplat(); })
            .UseEnvironment(
                Environments.Development
            )
            .Build();
    }

    public static IServiceCollection Services()
    {
        var services = new ServiceCollection();
        Services(services);
        return services;
    }

    public static void Services(IServiceCollection services)
    {
        services.UseMicrosoftDependencyResolver();
        var resolver = Locator.CurrentMutable;
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();


        DependencyBuilder.AddTtcServices(services);

        services.ReplaceFileSystem<MockFileSystem>();
    }
}