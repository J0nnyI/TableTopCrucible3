using ReactiveUI;

using Splat;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.DomainCore.WPF.Startup.PageViewModels;
using TableTopCrucible.DomainCore.WPF.Startup.Services;

namespace TableTopCrucible.DomainCore.WPF.Startup.WindowViewModels
{
    [Singleton(typeof(LauncherWindowVM))]
    public interface ILauncherWindow:IScreen
    {
        void Close();
    }
    public class LauncherWindowVM : ReactiveObject, IActivatableViewModel, ILauncherWindow
    {
        public void Close()
        {

        }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public Subject<Unit> _closeRequested = new Subject<Unit>();
        private readonly ILauncherService _launcherService;

        public IObservable<Unit> CloseRequested => _closeRequested;
        public RoutingState Router { get; } = new RoutingState();

        public ReactiveCommand<Unit, Unit> NavigateBack => Router.NavigateBack;

        public LauncherWindowVM(ILauncherService launcherService, IStartupPage startupPage)
        {
            Router.Navigate.Execute(startupPage);
            
            _launcherService = launcherService;
        }
    }
}
