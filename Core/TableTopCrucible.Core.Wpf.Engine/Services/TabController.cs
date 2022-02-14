using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using DynamicData;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Models;

namespace TableTopCrucible.Core.Wpf.Engine.Services
{
    // factory
    [Singleton]
    public interface ITabControllerFactory
    {
        public ITabController CreateTabController();
    }

    internal class TabControllerFactory : ITabControllerFactory
    {
        public ITabController CreateTabController()
        {
            return new TabController();

        }
    }

    public static class TabViewerExtensions
    {
        public static ReactiveCommand<ITabPage, Unit> CreateSelectCommand(this ITabViewer tabViewer)
            =>  ReactiveCommand.Create<ITabPage, Unit>(tab =>
            {
                RxApp.TaskpoolScheduler.Schedule(Unit.Default, (_,_) =>
                {
                    CompositeDisposable res = new();
                    tabViewer.Select(tab);
                    return res;
                });
                return Unit.Default;
            });
    }

    //can not edit the tabs (add/remove) but view and select them
    public interface ITabViewer
    {
        public IObservable<ITabPage> SelectedTabChanges { get; }
        public IObservableList<ITabPage> Tabs { get; }
        public void Select(ITabPage tab);
        public void SelectFirst();
    }
    // can add/remove tabs
    public interface ITabController:ITabViewer
    {
        void Add(params ITabPage[] tabs);
    }
    internal class TabController: ITabController
    {
        private readonly BehaviorSubject<ITabPage> _selectedTabChanges = new(null);
        public IObservable<ITabPage> SelectedTabChanges => _selectedTabChanges.WhereNotNull();
        public IObservableList<ITabPage> Tabs { get; }
        public void Select(ITabPage tab)
        {
            if (tab is null)
                throw new NullReferenceException(nameof(tab));
            if (!Tabs.Items.Contains(tab))
                throw new ArgumentException("the tab must be added before it can be selected");
            _selectedTabChanges.Value?.BeforeClose();
            _selectedTabChanges.OnNext(tab);

        }

        public void SelectFirst()
        {

            var selectedTab = Tabs.Items
                .Where(tab => tab.IsSelectable)
                .OrderBy(tab => tab.Position)
                .FirstOrDefault();
            if (selectedTab is not null)
                Select(selectedTab);
        }

        public void Add(params ITabPage[] tabs)
        {
            _tabs.AddRange(tabs);

            if (_selectedTabChanges.Value is null)
                SelectFirst();
        }

        private readonly SourceList<ITabPage> _tabs = new();

        public TabController()
        {
            Tabs = _tabs
                .Connect()
                .FilterOnObservable(tab=>tab.WhenAnyValue(tab=>tab.IsSelectable))
                .DisposeMany()
                .Sort(tab => tab.Position.Value)
                .AsObservableList();
        }
    }
}
