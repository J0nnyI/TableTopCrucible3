using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Core.DI
{
    public static class Factory
    {
        public static void GenerateVmMap()
        {

        }


        public static IServiceProvider GenerateServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();

            var types =
                AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes());

            services.TryAddEnumerable(getCollectionForAttribute<TransientAttribute>(types, ServiceLifetime.Transient));
            services.TryAddEnumerable(getCollectionForAttribute<SingletonAttribute>(types, ServiceLifetime.Singleton));
            services.TryAddEnumerable(getCollectionForAttribute<ScopedAttribute>(types, ServiceLifetime.Scoped));

            return services.BuildServiceProvider();
        }
        private static IEnumerable<ServiceDescriptor> getCollectionForAttribute<T>(IEnumerable<Type> types, ServiceLifetime lifetime) where T : IServiceAttribute
        {
            return types
                .Select(type => new { type, attribute = (T)type.GetCustomAttributes(typeof(T), true).FirstOrDefault()})
                .Where(typeEx => typeEx.attribute != null)
                .Select(typeEx => new ServiceDescriptor(typeEx.type, typeEx.attribute.Implementation, lifetime));
        }
    }
}
