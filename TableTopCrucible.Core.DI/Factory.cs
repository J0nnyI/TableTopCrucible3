using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.DI
{
    public static class Factory
    {
        public static IServiceProvider GenerateServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();
            try
            {
                var assemblies = AssemblyHelper.GetSolutionAssemblies();
                var types = assemblies
                        .SelectMany(assembly => assembly.DefinedTypes);

                var transients = getCollectionForAttribute<TransientAttribute>(types, ServiceLifetime.Transient);
                var singletons = getCollectionForAttribute<SingletonAttribute>(types, ServiceLifetime.Singleton);
                var scoped = getCollectionForAttribute<ScopedAttribute>(types, ServiceLifetime.Scoped);

                services.TryAddEnumerable(transients);
                services.TryAddEnumerable(singletons);
                services.TryAddEnumerable(scoped);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return services.BuildServiceProvider();
        }
        private static IEnumerable<ServiceDescriptor> getCollectionForAttribute<T>(IEnumerable<Type> types, ServiceLifetime lifetime) where T : IServiceAttribute
        {
            return types
                .Select(type => new
                {
                    type,
                    attribute = (T)type.GetCustomAttributes(typeof(T), true).FirstOrDefault()
                })
                .Where(typeEx => typeEx.attribute != null)
                .Select(typeEx => new ServiceDescriptor(typeEx.type, typeEx.attribute.Implementation, lifetime));
        }
    }
}
