using System;
using System.Diagnostics;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Domain.Library.Wpf.Services
{

    [Singleton]
    public interface ILibraryService : ITabViewer
    {
        // the entire selection
        SourceList<Item> SelectedItems { get; }
        // null if either none or multiple items are selected
        IObservable<Item> SingleSelectedItemChanges { get; }
        Item SingleSelectedItem { get; }
        void AddTab(params ITabPage[] tabs);

    }
    internal class LibraryService :ReactiveObject, ILibraryService
    {
        private readonly ITabController _tabController;


        public LibraryService(ITabControllerFactory tabControllerFactory)
        {
            _tabController = tabControllerFactory.CreateTabController();
            SingleSelectedItemChanges = SelectedItems
                .Connect()
                .ToCollection()
                .Select(items => items.OnlyOrDefault())
                .Replay(1)
                .RefCount();
            _singleSelectedItem = SingleSelectedItemChanges.ToProperty(this, s => s.SingleSelectedItem);

            _tabController.Tabs.Connect()
                .FilterOnObservable(tab => 
                    tab.WhenAnyValue(tab => tab.IsSelectable))
                .Count()
                .Select(count => count<2?count:2)
                .Throttle(TimeSpan.FromMilliseconds(20))
                .DistinctUntilChanged()
                .Subscribe(_ => SelectFirst());
        }

        public SourceList<Item> SelectedItems { get; } = new ();
        public IObservable<Item> SingleSelectedItemChanges { get; private set; }
        private readonly ObservableAsPropertyHelper<Item> _singleSelectedItem;
        public Item SingleSelectedItem => _singleSelectedItem.Value;

        public void AddTab(params ITabPage[] tabs)
            => _tabController.Add(tabs);


        #region tab control

        public IObservable<ITabPage> SelectedTabChanges => _tabController.SelectedTabChanges;

        public IObservableList<ITabPage> Tabs => _tabController.Tabs;
        public void Select(ITabPage tab)
            => _tabController.Select(tab);

        public void SelectFirst()
            => _tabController.SelectFirst();

        #endregion

    }
}
