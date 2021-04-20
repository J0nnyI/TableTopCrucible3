using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using System.IO;
using System.Linq;
using System.Windows;

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

        private void configureAutomapper(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(services.AddSingleton);

                cfg.AddMaps(nameof(TableTopCrucible.Data.Library.DataTransfer));

            });


        }

        private ILoggerFactory buildLoggingFactory()
        {
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
                    path: logDir + "\\ttc-log.clef",
                    fileSizeLimitBytes: 10000000,
                    retainedFileCountLimit: 2,
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true);


                builder.AddSerilog(loggerConfig.CreateLogger());
            });
            return factory;

        }

        protected override void OnStartup(StartupEventArgs e)
        {


            this.Resources.MergedDictionaries.Add(ViewModelAttribute.GetTemplateDictionary());

            var services = Core.DI.DiAttributeCollector.GenerateServiceProvider();

            var loggingFactory = buildLoggingFactory();

            services.AddSingleton(typeof(ILoggerFactory), loggingFactory);
            configureAutomapper(services);

            var provider = services.BuildServiceProvider();


            ILogger msLogger = loggingFactory.CreateLogger(nameof(App));
            msLogger.LogInformation("DI initialized");


            new MainWindow()
            {
                Content = provider.GetRequiredService<IMainPage>()
            }.Show();



        }
    }
}
