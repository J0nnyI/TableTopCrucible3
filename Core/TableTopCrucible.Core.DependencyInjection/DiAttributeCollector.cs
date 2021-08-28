﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TableTopCrucible.Core.Helper;
using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCtucible.Core.DependencyInjection
{
    public static class DiAttributeCollector
    {
        public static IServiceCollection GenerateServiceProvider(IServiceCollection services = null)
        {
            services ??= new ServiceCollection();

            var types = AssemblyHelper.GetSolutionTypes()
                    .ToArray();

            var transients = getCollectionForAttribute<TransientAttribute>(types, ServiceLifetime.Transient);
            var singletons = getCollectionForAttribute<SingletonAttribute>(types, ServiceLifetime.Singleton);
            var scoped = getCollectionForAttribute<ScopedAttribute>(types, ServiceLifetime.Scoped);

            services.TryAddEnumerable(transients);
            services.TryAddEnumerable(singletons);
            services.TryAddEnumerable(scoped);

            return services;
        }
        private static IEnumerable<ServiceDescriptor> getCollectionForAttribute<T>(IEnumerable<Type> types, ServiceLifetime lifetime) where T : IServiceAttribute
        {
            return types
                .Select(type => new
                {
                    type,
                    attribute = (T)type.GetCustomAttributes(typeof(T), false).FirstOrDefault()
                })
                .Where(typeEx => typeEx.attribute != null)
                .Select(typeEx => new ServiceDescriptor(typeEx.type, typeEx.attribute.Implementation, lifetime));
        }
    }
}
