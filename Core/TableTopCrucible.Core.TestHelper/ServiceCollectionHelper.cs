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
    }
}