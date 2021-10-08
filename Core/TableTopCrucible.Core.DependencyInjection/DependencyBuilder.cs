using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

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

        public static void GetServices(IServiceCollection services, bool includeAutoMapper = true)
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            services.TryAddEnumerable(DiAttributeCollector.GenerateServiceProvider());
            if (includeAutoMapper)
                configureAutoMapper(services);
            services.AddSingleton(typeof(ILoggerFactory), buildLoggingFactory());
        }

        public static IServiceProvider BuildDependencyProvider() => GetServices().BuildServiceProvider();

        private static void configureAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.Load("TableTopCrucible.Infrastructure.Repositories"));
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