using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using DynamicData;
using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Transient]
    public interface IIconTabStrip
    {
        void Init(ITabViewer tabViewer);
    }
    public class IconTabStripVm : ReactiveObject, IActivatableViewModel, IIconTabStrip
    {


        private ITabViewer _tabViewer;
        private ReadOnlyObservableCollection<TabSelectionInfo> _tabs;
        public ReadOnlyObservableCollection<TabSelectionInfo> Tabs => _tabs;

        public void Init(ITabViewer tabViewer)
        {
            this._tabViewer = tabViewer;
        }
        public ViewModelActivator Activator { get; } = new();

        public IconTabStripVm()
        {
            ReactiveCommand<ITabPage, Unit> selectCommand;
            this.WhenActivated(() => new[]
            {
                selectCommand = _tabViewer.CreateSelectCommand(),
                _tabViewer.Tabs
                    .Connect()
                    .Transform(tab=>new TabSelectionInfo(tab, _tabViewer.SelectedTabChanges,selectCommand))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _tabs)
                    .Subscribe(),
            });
        }
        
    }
}
