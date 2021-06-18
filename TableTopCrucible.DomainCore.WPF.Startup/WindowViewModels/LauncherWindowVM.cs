using ReactiveUI;

using System;
using System.Reactive;
using System.Reactive.Subjects;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.DomainCore.WPF.Startup.PageViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.WindowViewModels
{
    [Singleton(typeof(LauncherWindowVM))]
    public interface ILauncherWindow : IScreen
    {
    }
    public class LauncherWindowVM : ReactiveObject, IActivatableViewModel, ILauncherWindow
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public Subject<Unit> _closeRequested = new Subject<Unit>();

        public IObservable<Unit> CloseRequested => _closeRequested;
        public RoutingState Router { get; } = new RoutingState();

        public ReactiveCommand<Unit, Unit> NavigateBack => Router.NavigateBack;

        public LauncherWindowVM(IStartupPage startupPage)
        {
            Router.Navigate.Execute(startupPage);
        }
    }
}
