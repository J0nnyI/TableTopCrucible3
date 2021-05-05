using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

using TableTopCrucible.Core.WPF.Helper.Attributes;

namespace TableTopCrucible.App.Shared
{
    public class DependencyBuilder
    {
        public static IServiceProvider BuildDependencyProvider()
        {
            var services = Core.DI.DiAttributeCollector.GenerateServiceProvider();

            var loggingFactory = buildLoggingFactory();

            services.AddSingleton(typeof(ILoggerFactory), loggingFactory);
            configureAutomapper(services);

            return services.BuildServiceProvider();
        }
        private static void configureAutomapper(IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.Load("TableTopCrucible.Data.Library.DataTransfer"));
        }

        private static ILoggerFactory buildLoggingFactory()
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
    }
}
