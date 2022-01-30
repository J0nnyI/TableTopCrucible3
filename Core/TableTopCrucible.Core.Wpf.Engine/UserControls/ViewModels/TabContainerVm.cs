using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using DynamicData;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Transient]
    public interface ITabContainer
    {
        public void Init(ITabViewer viewer);
    }
    public class TabContainerVm : ReactiveObject, IActivatableViewModel, ITabContainer
    {
        private ITabViewer _tabViewer;
        public ViewModelActivator Activator { get; } = new();
        private ReadOnlyObservableCollection<TabSelectionInfo> _tabs;
        public ReadOnlyObservableCollection<TabSelectionInfo> Tabs => _tabs;

        public TabContainerVm()
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

        public void Init(ITabViewer viewer)
        {
            _tabViewer = viewer;
        }
    }
}
