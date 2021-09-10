using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Models;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton(typeof(NavigationService))]
    public interface INavigationService
    {
        INavigationPage CurrentPage { get; set; }
        IObservableList<INavigationPage> Pages { get; }
    }
    internal class NavigationService :ReactiveObject, INavigationService
    {
        private readonly SourceList<INavigationPage> _pages = new();
        public IObservableList<INavigationPage> Pages => _pages;
        [Reactive]
        public INavigationPage CurrentPage { get; set; }


        public NavigationService()
        {
            _pages.AddRange(
                DependencyInjectionHelper.GetServicesByType<INavigationPage>().Where(s=>s != null)// filter vm utilities
            );
        }
    }
}
