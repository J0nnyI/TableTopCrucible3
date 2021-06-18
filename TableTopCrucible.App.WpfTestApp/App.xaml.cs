using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ReactiveUI;

using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

using System;
using System.Reflection;
using System.Windows;

using TableTopCrucible.App.Shared;
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


                    DependencyBuilder.GetServices(services);
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSplat();
                })
                .UseEnvironment(Environments.Development)
                .Build();
            Container = host.Services;
            //Container.UseMicrosoftDependencyResolver();

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());


            var value = Container.GetRequiredService<IMain>();
            new Window()
            {
                Title = "TTC Tester",
                Content = new ViewModelViewHost()
                {
                    ViewModel = value,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                },
                VerticalContentAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            }.Show();

        }

    }
}
