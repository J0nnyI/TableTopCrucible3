using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Navigation.Models;
using TableTopCrucible.Core.WPF.Navigation.Views;

namespace TableTopCrucible.Core.WPF.Navigation.ViewModels
{
    [Singleton(typeof(NavigationContainerVM))]
    public interface INavigationContainer
    {
        void Initialize(INavigationProvider navigationProvider);
    }

    
    public class NavigationContainerVM : INavigationContainer
    {
        private BehaviorSubject<INavigationProvider> _navigationProvider = new BehaviorSubject<INavigationProvider>(null);
        public NavigationContainerVM()
        { }

        public void Initialize(INavigationProvider navigationProvider)
        {
            if (_navigationProvider.Value != null)
                throw new InvalidOperationException("the service has already been initialized");

            this._navigationProvider.OnNext(navigationProvider ?? throw new ArgumentNullException(nameof(navigationProvider)));
        }
    }
}