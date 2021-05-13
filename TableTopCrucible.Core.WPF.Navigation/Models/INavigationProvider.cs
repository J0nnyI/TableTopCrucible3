using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TableTopCrucible.Core.WPF.Navigation.Models
{
    public interface INavigationProvider
    {
        IObservableList<INavigationPage> History { get; }
        INavigationPage CurrentPage { get; }

        void NavigateTo(INavigationPage page);
    }
    public abstract class NavigationServiceBase : ReactiveObject, INavigationProvider
    {
        private readonly SourceList<INavigationPage> _history = new SourceList<INavigationPage>();
        public IObservableList<INavigationPage> History => _history;
        [Reactive]
        public INavigationPage CurrentPage { get; private set; }
        public void NavigateTo<T>(INavigationPage<T> page)
        {

        }
        public void NavigateTo(INavigationPage page)
        {
            _history.Add(page);
            this.CurrentPage = page;
        }
    }
}
