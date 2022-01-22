using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Splat;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TableTopCrucible.Core.DependencyInjection
{
    public static class DependencyBuilder
    {
        public static void AddTtcServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            services.TryAddEnumerable(DiAttributeCollector.GenerateServiceProvider());
            services.AddSingleton(typeof(ILoggerFactory), buildLoggingFactory());
        }


        private static ILoggerFactory buildLoggingFactory()
        {
            var logDir = "logging";
            if (Directory.Exists(logDir))
                Directory.GetFiles(logDir).ToList().ForEach(File.Delete);
            var factory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Warning);
                var loggerConfig = new LoggerConfiguration()
                    .MinimumLevel.Is(LogEventLevel.Warning)
                    .WriteTo.Debug()
                    .WriteTo.File(
                        new CompactJsonFormatter(),
                        logDir + "\\ttc-log.clef",
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