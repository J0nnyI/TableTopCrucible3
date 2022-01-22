using System;
using System.Collections.Generic;
using System.Linq;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.DependencyInjection.Exceptions;
using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.DependencyInjection
{
    public static class DiAttributeCollector
    {
        public static IServiceCollection GenerateServiceProvider(IServiceCollection services = null)
        {
            services ??= new ServiceCollection();


            var classes = AssemblyHelper.GetSolutionClasses().ToArray();

            var transients = getCollectionForAttribute<TransientAttribute>(classes);
            var singletons = getCollectionForAttribute<SingletonAttribute>(classes);
            var scoped = getCollectionForAttribute<ScopedAttribute>(classes);

            services.TryAddEnumerable(transients);
            services.TryAddEnumerable(singletons);
            services.TryAddEnumerable(scoped);

            var faultyServices = services
                .Where(service => service.ImplementationType == null || service.ServiceType == null).ToArray();
            if (faultyServices.Any()) throw new IncompleteServiceException(faultyServices);


            return services;
        }

        private static IEnumerable<ServiceDescriptor> getCollectionForAttribute<T>(IEnumerable<Type> classes)
            where T : Attribute
        {
            var lifetime = ServiceLifetime.Singleton;
            if (typeof(T) == typeof(ScopedAttribute))
                lifetime = ServiceLifetime.Scoped;
            if (typeof(T) == typeof(TransientAttribute))
                lifetime = ServiceLifetime.Transient;

            return AssemblyHelper.GetSolutionTypesByAttribute<T>()
                .Select(@interface => new
                {
                    @interface,
                    @class = classes.First(@class => @class.IsAssignableTo(@interface))
                }).Select(service =>
                    new ServiceDescriptor(service.@interface, service.@class, lifetime)
                );
        }
    }
}