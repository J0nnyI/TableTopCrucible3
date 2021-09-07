using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Models;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton(typeof(NavigationService))]
    public interface INavigationService
    {
        IObservableList<INavigationPage> Pages { get; }
    }
    internal class NavigationService : INavigationService
    {
        private readonly SourceList<INavigationPage> _pages = new();
        public IObservableList<INavigationPage> Pages => _pages;


        public NavigationService()
        {
            _pages.AddRange(
                DependencyInjectionHelper.GetServicesByType<INavigationPage>()
            );
        }
        public void Navigate<T>()
            where T : class, INavigationService
        {
            throw new NotImplementedException(nameof(Navigate));
        }
    }
}
