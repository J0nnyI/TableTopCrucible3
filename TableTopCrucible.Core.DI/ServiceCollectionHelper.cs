using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TableTopCrucible.Core.DI
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
            var automapperAssembly = Assembly.GetAssembly(typeof(Mapper));
            srv.Where(desc => desc.ServiceType.Assembly == automapperAssembly)
                .Select(desc => desc.ServiceType)
                .ToList()
                .ForEach(asrv => srv.RemoveAll(asrv));
        }
        
    }
}
