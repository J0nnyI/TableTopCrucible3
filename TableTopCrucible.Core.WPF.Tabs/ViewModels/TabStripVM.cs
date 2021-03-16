using DynamicData;

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

using TableTopCrucible.Core.WPF.Tabs.Models;

using TableTopCurcible.Core.BaseUtils;

namespace TableTopCrucible.Core.WPF.Tabs.ViewModels
{
    public interface ITabStripVM
    {
        TabModel CurrentTab { get; }
        public IObservableList<TabModel> Tabs { get; }
    }
    internal class TabStripVM : DisposableReactiveObjectBase, ITabStripVM
    {
        private readonly SourceList<TabModel> _tabs = new SourceList<TabModel>();
        public IObservableList<TabModel> Tabs => _tabs;
        public TabModel CurrentTab { get; private set; }
        public TabStripVM()
        {
            _tabs.DisposeWith(disposables);
            _tabs.Connect().DisposeMany().TakeUntil(Destroy).Subscribe();
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
