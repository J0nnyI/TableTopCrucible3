using MaterialDesignThemes.Wpf;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCurcible.Core.BaseUtils;

namespace TableTopCrucible.Core.WPF.Tabs.Models
{
    public class TabModel : DisposableReactiveObjectBase
    {
        public TabModel(string title, Func<object> viewModelFactory)
        {
            this.ViewModelFactory = viewModelFactory;
            this.Title = title;
        }
        protected override void onDispose()
        {
            base.onDispose();
            if (ViewModel is IDisposable disposableVM)
                disposableVM.Dispose();
        }

        [Reactive]
        public string Title { get; set; }
        [Reactive]
        public object ViewModel { get; private set; }
        public Func<object> ViewModelFactory { get; }
        [Reactive]
        public PackIcon Icon { get; set; }
        [Reactive]
        public int TabIndex { get; set; }
        /// i.e. settings, library or mapEditor
        public string Kind { get; set; }
        public bool KeepLoaded { get; set; } = true;
    }
}
