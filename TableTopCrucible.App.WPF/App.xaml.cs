using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

using TableTopCrucible.App.Shared;
using TableTopCrucible.App.WPF.ViewModels;
using TableTopCrucible.Core.WPF.Helper.Attributes;

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


            new MainWindow()
            {
                Content = di.GetRequiredService<IMainPage>()
            }.Show();



        }
    }
}
