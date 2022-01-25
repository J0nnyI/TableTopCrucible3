using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls
{
    [Transient]
    public interface IItemListFilter
    {
        IObservable<Func<Item,bool>> FilterChanges { get; }
    }

    public class ItemListFilterVm : ReactiveObject, IItemListFilter, IActivatableViewModel
    {
        public ITagEditor IncludeTags { get; }
        public ITagEditor ExcludeTags { get; }
        public ViewModelActivator Activator { get; } = new();
        private readonly SourceList<Tag> _includedTagList  = new();
        private readonly SourceList<Tag> _excludedTagList  = new();
        public IObservable<Func<Item, bool>> FilterChanges { get; }
        public ItemListFilterVm(ITagEditor includeTags, ITagEditor excludeTags)
        {
            IncludeTags = includeTags;
            ExcludeTags = excludeTags;
            IncludeTags.TagSource = _includedTagList;
            ExcludeTags.TagSource = _excludedTagList;
            includeTags.FluentModeEnabled = excludeTags.FluentModeEnabled = false;

            FilterChanges =
                Observable.CombineLatest(
                    _includedTagList
                    .Connect()
                    .StartWithEmpty()
                    .ToCollection(),

                    _excludedTagList
                        .Connect()
                        .StartWithEmpty()
                        .ToCollection(),
                    
                    (includedTags, excludedTags) =>
                    {
                        var itemUpdater = new BehaviorSubject<Unit>(Unit.Default);
                        Item item_ = null;
                        return itemUpdater.Select(_=> 
                           new
                           {
                               includedTags,
                               excludeTags,
                               item_,
                               filter=new Func<Item, bool>(item =>
                               {
                                   item_ = item;
                                   item.Tags.Connect().Subscribe(_ => itemUpdater.OnNext());
                                   var itemTags = item.Tags.Items;
                                   var includePassed = includedTags.All(filterTag => itemTags.Contains(filterTag));
                                   var excludePassed = excludedTags.None(filterTag => itemTags.Contains(filterTag));
                                   return includePassed && excludePassed;
                               })
                           }
                        );
                    })
                .Switch()
                .Select(x=>x.filter)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Publish()
                .RefCount();
        }

    }
}