using System;
using System.Collections.Generic;
using System.Linq;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.DependencyInjection
{
    public static class DependencyInjectionHelper
    {
        public static IEnumerable<Type> GetServiceTokensOfType<T>()
        {
            return AssemblyHelper.GetSolutionClassesOfType<T>()
                !.Select(t =>
                    t.GetInterfaces()
                        .FirstOrDefault(it => // get the actual type which we can use to inject the instance
                            it.HasCustomAttribute<SingletonAttribute>()
                            || it.HasCustomAttribute<TransientAttribute>()
                            || it.HasCustomAttribute<ScopedAttribute>()));
        }

        public static IEnumerable<T> GetServicesByType<T>()
        {
            return GetServiceTokensOfType<T>()
                .Select(t => (T)Locator.Current.GetService(t));
        }
    }
}