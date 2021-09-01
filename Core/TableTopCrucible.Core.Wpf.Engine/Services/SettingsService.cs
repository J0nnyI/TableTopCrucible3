using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

using DynamicData;
using Microsoft.AspNetCore.Components.Routing;
using ReactiveUI;

using Splat;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton(typeof(SettingsService))]
    public interface ISettingsService
    {
        IObservableCache<ISettingsCategoryPage, Name> Pages { get; }
    }

    internal class SettingsService : ISettingsService
    {
        private readonly SourceCache<ISettingsCategoryPage, Name> _pages = new(page => page.Title);
        public IObservableCache<ISettingsCategoryPage, Name> Pages => _pages;


        public SettingsService()
        {
            _initPages();
        }

        private void _initPages()
        {
            _pages
                .AddOrUpdate(
                    AssemblyHelper.GetSolutionTypes()
                        !.Where(t => t.IsAssignableTo(typeof(ISettingsCategoryPage)) && t.IsClass)
                        !.Select(t => (ISettingsCategoryPage)
                            Locator.Current.GetService(
                                t.GetInterfaces()
                                    .FirstOrDefault(it => // get the actual type which we can use to inject the instance
                                        it.HasCustomAttribute<SingletonAttribute>()
                                        || it.HasCustomAttribute<TransientAttribute>()
                                        || it.HasCustomAttribute<ScopedAttribute>())))
                        !.OrderBy(vm=>vm.Position)
                        !.ToArray()
                );
        }

    }
}