using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.EventLog;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TableTopCrucible.App.Shared;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TableTopCrucible.App.WpfTestApp.ViewModels;

namespace TableTopCrucible.App.WpfTestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Container { get; private set; }
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
                    

                    services.TryAddEnumerable(DependencyBuilder.GetServices());
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSplat();
                })
                .UseEnvironment(Environments.Development)
                .Build();
            Container = host.Services;
            Container.UseMicrosoftDependencyResolver();

            new Window()
            {
                Title="TTC Tester",
                Content = Container.GetRequiredService<IMain>()
            }.Show();

        }

    }
}
