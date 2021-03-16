using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.WPF.Tabs.ViewModels;

namespace TableTopCrucible.Core.WPF.Tabs
{
    public static class DependencyInjectionProvider
    {
        public static IServiceCollection Get()
        {
            IServiceCollection collection = new ServiceCollection();
            collection.AddTransient<ITabStripVM, TabStripVM>();

            return collection;
        }
    }
}
