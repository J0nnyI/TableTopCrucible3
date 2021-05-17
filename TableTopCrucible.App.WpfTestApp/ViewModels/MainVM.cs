using ReactiveUI;

using Splat;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;

using TableTopCrucible.App.WpfTestApp.Pages;
using TableTopCrucible.App.WpfTestApp.PageViewModels;
using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.App.WpfTestApp.ViewModels
{
    [Transient(typeof(MainVM))]
    interface IMain
    {

        static IMain()
        {

        }
    }
    public class MainVM : ReactiveObject, IActivatableViewModel, IMain
    {
        public RoutingState Router { get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }
        public ReactiveCommand<Unit, Unit> GoBack { get; }

        public MainVM()
        {
            GoNext = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute( new FirstViewModel()));
            GoBack = Router.NavigateBack;
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
