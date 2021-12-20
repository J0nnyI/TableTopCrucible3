using System.IO.Abstractions;
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