using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Serilog.Sinks.File;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using TableTopCrucible.App.WPF.ViewModels;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.Core.WPF.Tabs.ViewModels;

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

            var services = Core.DI.DiAttributeCollector.GenerateServiceProvider();

            var logDir = "logging";
            if (Directory.Exists(logDir)) 
            Directory.GetFiles(logDir).ToList().ForEach(File.Delete);
            ILoggerFactory factory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Warning);
                var loggerConfig = new LoggerConfiguration()
                 .MinimumLevel.Is(LogEventLevel.Warning)
                 .WriteTo.Debug()
                 .WriteTo.File(
                    formatter: new CompactJsonFormatter(),
                    path: logDir +"\\ttc-log.clef",
                    fileSizeLimitBytes: 10000000,
                    retainedFileCountLimit: 2,
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true);


                builder.AddSerilog(loggerConfig.CreateLogger());
            });

            services.AddSingleton(typeof(ILoggerFactory), factory);

            var provider = services.BuildServiceProvider();


            ILogger msLogger = factory.CreateLogger(nameof(App));
            msLogger.LogInformation("DI initialized");


            new MainWindow()
            {
                Content = provider.GetRequiredService<IMainPage>()
            }.Show();



        }
    }
}
