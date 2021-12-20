using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Linq;
using TableTopCrucible.Core.DependencyInjection;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.Models;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    [Singleton]
    public interface INavigationService
    {
        INavigationPage ActiveWorkArea { get; set; }
        ISidebarPage ActiveSidebar { get; set; }
        IObservableList<INavigationPage> Pages { get; }
        bool IsNavigationExpanded { get; set; }
    }

    internal class NavigationService : ReactiveObject, INavigationService
    {
        private readonly SourceList<INavigationPage> _pages = new();
        public IObservableList<INavigationPage> Pages => _pages;

        [Reactive]
        public bool IsNavigationExpanded { get; set; } = true;

        [Reactive]
        public INavigationPage ActiveWorkArea { get; set; }

        [Reactive]
        public ISidebarPage ActiveSidebar { get; set; }

        public NavigationService()
        {
            _pages.AddRange(
                DependencyInjectionHelper.GetServicesByType<INavigationPage>()
                    .Where(s => s != null) // filter vm utilities
            );
        }
    }
}