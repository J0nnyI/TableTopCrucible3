using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.Wpf.Engine.Models;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    public class TabSelectionInfo : ReactiveObject, IDisposable
    {
        private readonly ObservableAsPropertyHelper<PackIconKind> _icon;
        public PackIconKind Icon => _icon.Value;
        public ITabPage Content { get; }
        private readonly CompositeDisposable _disposables = new();
        private readonly ObservableAsPropertyHelper<bool> _isSelected;

        public bool IsSelected
        {
            get=>_isSelected. Value;
            set
            {
                if (IsSelected == value)
                    return;

                OnClickCommand.Execute(
                    value
                    ? Content
                    : null);
            }
        }
        public void Dispose() => _disposables.Dispose();
        public ReactiveCommand<ITabPage, Unit> OnClickCommand { get; }

        public TabSelectionInfo(ITabPage content, IObservable<ITabPage> selectedTabChanges, ReactiveCommand<ITabPage, Unit> onClickCommand)
        {
            Content = content;
            OnClickCommand = onClickCommand;
            _isSelected = selectedTabChanges
                .Select(selTab => selTab == content)
                .DistinctUntilChanged()
                .Do(x=>{})
                .ToProperty(this, vm => vm.IsSelected, false, RxApp.MainThreadScheduler);
            _icon = selectedTabChanges
                .Select(selTab => selTab == content
                    ? content.SelectedIcon
                    : content.UnselectedIcon)
                .DistinctUntilChanged()
                .ToProperty(this, vm => vm.Icon,false, RxApp.MainThreadScheduler)
                .DisposeWith(_disposables);
        }

    }
}