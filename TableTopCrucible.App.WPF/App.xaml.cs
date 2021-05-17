using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ReactiveUI;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using Splat;

using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

using TableTopCrucible.App.Shared;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.Core.WPF.MainWindow.ViewModels;
using TableTopCrucible.DomainCore.WPF.Startup.PageViewModels;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TableTopCrucible.App.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.Resources.MergedDictionaries.Add(ViewModelAttribute.GetTemplateDictionary());

            var di = DependencyBuilder.BuildDependencyProvider();

            ILogger msLogger = di.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(App));
            msLogger.LogInformation("DI initialized");
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());

            new MainWindow()
            {
                Content = di.GetRequiredService<IMainPage>()
            }.Show();



        }
    }
}
