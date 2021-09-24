using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using ReactiveUI;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TableTopCrucible.Core.DependencyInjection
{
    public class DependencyBuilder
    {
        public static IServiceCollection GetServices()
        {
            var services = new ServiceCollection();
            GetServices(services);
            return services;
        }
        public static void GetServices(IServiceCollection services, bool includeAutomapper = true)
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            services.TryAddEnumerable(DiAttributeCollector.GenerateServiceProvider());
            if(includeAutomapper)
            configureAutomapper(services);
            services.AddSingleton(typeof(ILoggerFactory), buildLoggingFactory());

        }
        public static IServiceProvider BuildDependencyProvider()
            => GetServices().BuildServiceProvider();
        private static void configureAutomapper(IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.Load("TableTopCrucible.Infrastructure.Repositories"));
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
