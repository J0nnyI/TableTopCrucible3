using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Shared.Wpf.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

public class MultiItemTagManager : ITagManager
{
    private readonly IObservableList<Item> _itemListChanges;

    private class AccumulatorHelper
    {
        public Dictionary<Tag, int> TagCounter { get; } = new();
        public HashSet<ItemId> ItemIds { get; } = new();
    }

    public MultiItemTagManager(IObservableList<Item> itemListChanges)
    {
        _itemListChanges = itemListChanges;
        Tags = itemListChanges
            .Connect()
            .TransformMany(item => item
                .Tags
                .Connect()
                .Transform(tag => new { tag, item.Id })
                .AsObservableList())
            .Throttle(SettingsHelper.DefaultThrottle)
            .ToCollection()
            .Select(tags => {
                var agg = Enumerable.Aggregate(tags, new AccumulatorHelper(), 
                    (acc, itemInfo) =>
                    {
                        if (!acc.ItemIds.Contains(itemInfo.Id))
                            acc.ItemIds.Add(itemInfo.Id);

                        if (!acc.TagCounter.TryGetValue(itemInfo.tag, out var count))
                            acc.TagCounter.Add(itemInfo.tag, 1);
                        else
                            acc.TagCounter[itemInfo.tag] = ++count;

                        return acc;
                    },
                    acc =>
                    {
                        var itemCnt = acc.ItemIds.Count;
                        if (acc.TagCounter.Count < itemCnt)
                        {

                        }
                        return acc.TagCounter.Select(cnt => FractionTag.From(
                                cnt.Key,
                                (Fraction)((double)cnt.Value / (double)itemCnt)));
                    }).ToArray();
                return agg;
            });
    }

    public void Add(Tag tag)
        => _itemListChanges.Items.ToList().ForEach(item => item.Tags.Add(tag));

    public void Remove(Tag tag)
        => _itemListChanges.Items.ToList().ForEach(item => item.Tags.Remove(tag));

    public void Replace(Tag oldTag, Tag newTag)
        => _itemListChanges.Items.ToList().ForEach(item => item.Tags.Replace(oldTag,newTag));
    
    public IObservable<IEnumerable<FractionTag>> Tags { get; }

    public void Dispose()
    {
    }
}