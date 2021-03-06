using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System.IO.Abstractions;
using System.Linq;
using System.Reflection;

namespace TableTopCrucible.Core.DependencyInjection
{
    public static class ServiceCollectionHelper
    {
        public static void ReplaceFileSystem<T>(this ServiceCollection srv) where T : class, IFileSystem
        {
            srv.RemoveAll<IFileSystem>();
            srv.AddSingleton<IFileSystem, T>();
        }
        public static void RemoveAutoMapper(this ServiceCollection srv)
        {
            var autoMapperAssembly = Assembly.GetAssembly(typeof(Mapper));
            srv.Where(desc => desc.ServiceType.Assembly == autoMapperAssembly)
                .Select(desc => desc.ServiceType)
                .ToList()
                .ForEach(asrv => srv.RemoveAll(asrv));
        }

    }
}
