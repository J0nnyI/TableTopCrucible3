using ReactiveUI;

using Splat;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.DomainCore.WPF.Startup.WindowViewModels;
using TableTopCrucible.DomainCore.WPF.Startup.WindowViews;

namespace TableTopCrucible.DomainCore.WPF.Startup.Services
{
    [Singleton(typeof(LauncherService))]
    public interface ILauncherService
    {
        ILauncherWindow OpenLauncher();
        internal IScreen Screen { get; }
    }

    internal class LauncherService : ILauncherService
    {
        public ILauncherWindow OpenLauncher()
        {
            var win = new LauncherWindow();
            win.Show();
            return win.ViewModel;
        }

        private readonly Lazy<IScreen> _screen = new Lazy<IScreen>(
            () => Locator.Current.GetService<ILauncherWindow>());
        internal IScreen Screen => _screen.Value;
        IScreen ILauncherService.Screen => Screen;
    }
}
