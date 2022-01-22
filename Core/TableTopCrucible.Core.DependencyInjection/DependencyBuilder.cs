using System.IO;
using System.Linq;
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