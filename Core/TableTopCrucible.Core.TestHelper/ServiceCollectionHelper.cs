using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TableTopCrucible.Core.TestHelper
{
    public static class ServiceCollectionHelper
    {
        public static void ReplaceFileSystem<T>(this IServiceCollection srv) where T : class, IFileSystem
        {
            srv.RemoveAll<IFileSystem>();
            srv.AddSingleton<IFileSystem, T>();
        }

        public static void RemoveAutoMapper(this ServiceCollection serviceCollection)
        {
            var autoMapperAssembly = Assembly.GetAssembly(typeof(Mapper));
            serviceCollection.Where(serviceDescriptor => serviceDescriptor.ServiceType.Assembly == autoMapperAssembly)
                .Select(serviceDescriptor => serviceDescriptor.ServiceType)
                .ToList()
                .ForEach(serviceType => serviceCollection.RemoveAll(serviceType));
        }
    }
}