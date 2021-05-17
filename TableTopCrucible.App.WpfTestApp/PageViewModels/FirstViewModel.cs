using ReactiveUI;

using Splat;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.App.WpfTestApp.Pages;

namespace TableTopCrucible.App.WpfTestApp.PageViewModels
{
    public class FirstViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string UrlPathSegment => "first";

        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public FirstViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        }
    }
}
