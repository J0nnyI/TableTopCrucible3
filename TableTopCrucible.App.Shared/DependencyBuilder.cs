using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using static TableTopCrucible.Core.BaseUtils.FileSystemHelper;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using System;
using System.Linq;
using System.Reflection;
using System.IO.Abstractions;

namespace TableTopCrucible.App.Shared
{
    public class DependencyBuilder
    {
        public static IServiceCollection GetServices()
        {
            var services = new ServiceCollection();
            GetServices(services);
            return services;
        }
        public static void GetServices(IServiceCollection services)
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            services.TryAddEnumerable(Core.DI.DiAttributeCollector.GenerateServiceProvider());
            configureAutomapper(services);
            services.AddSingleton(typeof(ILoggerFactory), buildLoggingFactory());
        }
        public static IServiceProvider BuildDependencyProvider()
            => GetServices().BuildServiceProvider();
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
