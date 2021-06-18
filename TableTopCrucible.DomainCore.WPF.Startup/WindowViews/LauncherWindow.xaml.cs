using ReactiveUI;

using Splat;

using System;
using System.Reactive.Disposables;

using TableTopCrucible.DomainCore.WPF.Startup.WindowViewModels;

namespace TableTopCrucible.DomainCore.WPF.Startup.WindowViews
{
    /// <summary>
    /// Interaction logic for LauncherWIndow.xaml
    /// </summary>
    public partial class LauncherWindow : ReactiveWindow<LauncherWindowVM>
    {
        public LauncherWindow()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                ViewModel ??= Locator.Current.GetService<ILauncherWindow>() as LauncherWindowVM;
                ViewModel
                    .CloseRequested
                    .Subscribe(_ => this.Close())
                    .DisposeWith(disposables);


                this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
                    .DisposeWith(disposables);
                //this.BindCommand(ViewModel, x => x.GoNext, x => x.GoNextButton)
                //    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.NavigateBack, v => v.PreviousStep)
                    .DisposeWith(disposables);
            });
        }

    }
}
