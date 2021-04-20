using DynamicData;

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using TableTopCrucible.Core.BaseUtils;
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.Core.WPF.Tabs.Models;
using TableTopCrucible.Core.WPF.Tabs.Views;

namespace TableTopCrucible.Core.WPF.Tabs.ViewModels
{
    [Transient(typeof(TabStripVM))]
    public interface ITabStripVM
    {
        TabModel CurrentTab { get; }
        public IObservableList<TabModel> Tabs { get; }
    }

    [ViewModel(typeof(TabStripV))]
    internal class TabStripVM : DisposableReactiveObjectBase, ITabStripVM
    {
        private readonly SourceList<TabModel> _tabs = new SourceList<TabModel>();
        public IObservableList<TabModel> Tabs => _tabs;
        public TabModel CurrentTab { get; private set; }
        public TabStripVM()
        {
            _tabs.DisposeWith(disposables);
            _tabs.Connect()
                .DisposeMany()
                .TakeUntil(Destroy)
                .Subscribe();
        }
        public void SetCurrentTab(TabModel tab)
        {
            if (CurrentTab.KeepLoaded)
                CurrentTab.Dispose();
        }
        protected override void onDispose()
        {
            base.onDispose();
            if (CurrentTab?.KeepLoaded == true)
                this.CurrentTab?.Dispose();
        }
    }
}
