using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Controller;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls
{
    internal class AppendedDisposable<T> : IDisposable
    {
        public T Content { get; }
        private readonly CompositeDisposable _disposable;

        public AppendedDisposable(T content, CompositeDisposable disposable)
        {
            Content = content;
            _disposable = disposable;
        }

        public void Dispose() => _disposable.Dispose();
    }

    [Transient]
    public interface IItemListFilter
    {
        IObservable<Func<Item, bool>> FilterChanges { get; }
    }
    public class ItemListFilterVm : ReactiveObject, IItemListFilter, IActivatableViewModel
    {
        public IItemListFilterElements IncludeFilter { get; }
        public IItemListFilterElements ExcludeFilter { get; }
        public ViewModelActivator Activator { get; } = new();
        public IObservable<Func<Item, bool>> FilterChanges { get; }
        public ItemListFilterVm(IItemListFilterElements includeFilter, IItemListFilterElements excludeFilter, IItemRepository itemRepository)
        {
            IncludeFilter = includeFilter;
            IncludeFilter.FilterMode = FilterMode.Include;
            ExcludeFilter = excludeFilter;
            excludeFilter.FilterMode = FilterMode.Exclude;


            var filterUpdater = new Subject<Unit>();

            itemRepository
                .Data
                .Connect()
                .Transform(item =>
                {
                    return new AppendedDisposable<Item>(item, new(new IDisposable[]
                    {
                        item.WhenAnyPropertyChanged()
                            .Subscribe(_=>filterUpdater.OnNext()),
                        item.Tags.Connect().Subscribe(_=>filterUpdater.OnNext())
                    }));
                })
                .DisposeMany()
                .Subscribe();

            this.FilterChanges =
                filterUpdater
                    .StartWith(Unit.Default)
                    .CombineLatest(
                    IncludeFilter.FilterChanges,
                    ExcludeFilter.FilterChanges,
                    (_, filterIn, filterEx) =>
                        new Func<Item, bool>(item => 
                            filterIn(item) && filterEx(item))
                    );
        }

    }
}