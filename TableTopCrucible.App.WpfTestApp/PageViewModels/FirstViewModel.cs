using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

using TableTopCrucible.App.WpfTestApp.Pages;

namespace TableTopCrucible.App.WpfTestApp.PageViewModels
{
    public class FirstViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string UrlPathSegment => "first";

        public FirstViewModel()
        {
            ;
            this.WhenActivated((CompositeDisposable disposables) =>
            {

                SetText = ReactiveCommand.Create(
                        () => Value = "setByCommand",
                        this.WhenAnyValue(vm => vm.Value).Select(string.IsNullOrWhiteSpace),
                        RxApp.TaskpoolScheduler
                    ).DisposeWith(disposables);

                AddText = ReactiveCommand.Create(
                        (string suffix) => Value += suffix,
                        this.WhenAnyValue(vm => vm.SuffixFills),
                        RxApp.TaskpoolScheduler
                    ).DisposeWith(disposables);
            });
        }

        [Reactive]
        public string Value { get; set; }
        [Reactive]
        public bool SuffixFills { get; set; }
        public ReactiveCommand<Unit, string> SetText { get; private set; }
        public ReactiveCommand<string, string> AddText { get; private set; }

        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public FirstViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        }
    }
}
