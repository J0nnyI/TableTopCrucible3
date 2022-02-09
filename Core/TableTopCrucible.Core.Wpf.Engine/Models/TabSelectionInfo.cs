using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TableTopCrucible.Core.Wpf.Engine.Models
{
    public class TabSelectionInfo : ReactiveObject, IDisposable
    {
        [ObservableAsProperty]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public PackIconKind Icon { get; }
        public ITabPage Content { get; }
        private readonly CompositeDisposable _disposables = new();

        [ObservableAsProperty]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public bool IsSelected { get; }
        public void Dispose() => _disposables.Dispose();
        public ReactiveCommand<ITabPage, Unit> OnClickCommand { get; }

        public TabSelectionInfo(ITabPage content, IObservable<ITabPage> selectedTabChanges, ReactiveCommand<ITabPage, Unit> onClickCommand)
        {
            Content = content;
            OnClickCommand = onClickCommand;
            var isSelectedChanges = selectedTabChanges
                .Select(selTab => selTab == content)
                .DistinctUntilChanged();
            isSelectedChanges
                .ToPropertyEx(this, x => x.IsSelected)
                .DisposeWith(_disposables);
            isSelectedChanges.Subscribe(selected =>

                OnClickCommand.Execute(selected ? Content : null));

            selectedTabChanges
                            .Select(selTab => selTab == content
                    ? content.SelectedIcon
                    : content.UnselectedIcon)
                .DistinctUntilChanged()
                .ToPropertyEx(this, vm => vm.Icon, false, RxApp.MainThreadScheduler)
                .DisposeWith(_disposables);
        }

    }
}