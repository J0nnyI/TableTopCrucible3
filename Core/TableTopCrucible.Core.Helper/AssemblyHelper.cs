

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TableTopCrucible.Core.Helper
{
    public static class AssemblyHelper
    {
        // must not use System.IO.Abstractions since the DI builder is not yet done
        public static IEnumerable<Assembly> GetSolutionAssemblies()
        {
            return Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Where(file => Path.GetFileName(file).Contains("TableTopCrucible"))
                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)))
                .ToArray();
        }

        public static IEnumerable<Type> GetSolutionTypes()
            => GetSolutionAssemblies().SelectMany(a => a.GetTypes());

        public static IEnumerable<Type> GetSolutionTypes(Type baseType)
            => GetSolutionTypes().Where(t => t.IsAssignableFrom(baseType));

        public static IEnumerable<Type> GetSolutionTypesByAttribute<T>() where T : Attribute
            => GetSolutionTypes().Where(t => t.HasCustomAttribute<T>());

    }
}
