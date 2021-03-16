using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Linq;

using TableTopCrucible.Core.DI;

namespace TableTopCrucible.App.DI
{
    public static class DependencyBuilder
    {
        public static ServiceProvider Get()
        {
            ServiceCollection services = new ServiceCollection();
            //services.TryAddEnumerable(Core.WPF.Tabs.DependencyInjectionProvider.Get());



            return services.BuildServiceProvider();
        }
    }
}
