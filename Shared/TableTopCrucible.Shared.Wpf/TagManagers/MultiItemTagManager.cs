using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Shared.Wpf.Models.TagEditor;
using TableTopCrucible.Shared.Wpf.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

public class MultiItemTagManager : ITagManager
{
    private readonly IObservableList<Item> _itemListChanges;
    private readonly INotificationService _notificationService;
    private readonly IProgressTrackingService _progressTrackingService;

    private class AccumulatorHelper
    {
        public Dictionary<Tag, int> TagCounter { get; } = new();
        public HashSet<ItemId> ItemIds { get; } = new();
    }

    public MultiItemTagManager(IObservableList<Item> itemListChanges)
    {
        _notificationService = Locator.Current.GetService<INotificationService>();
        _progressTrackingService = Locator.Current.GetService<IProgressTrackingService>();
        _itemListChanges = itemListChanges;
        Tags = itemListChanges
            .Connect()
            .ObserveOn(RxApp.TaskpoolScheduler)
            .TransformMany(item => item
                .Tags
                .Connect()
                .Transform(tag => new { tag, item.Id })
                .AsObservableList())
            .Buffer(SettingsHelper.DefaultThrottle)
            .FlattenBufferResult()
            .ToCollection()
            .Select(tags => {
                var agg = tags.Aggregate(new AccumulatorHelper(), 
                    (acc, itemInfo) =>
                    {
                        acc.ItemIds.Add(itemInfo.Id);
                        acc.TagCounter.AddOrUpdate(itemInfo.tag, 1, tagCount=>tagCount+1);
                        return acc;
                    },
                    acc =>
                    {
                        var itemCnt = acc.ItemIds.Count;
                        return acc.TagCounter.Select(TagCounter =>
                        {
                            var tagCount = TagCounter.Value;
                            var tag = TagCounter.Key;
                            
                            if (tagCount > itemCnt)
                                Debugger.Break();

                            return FractionTag.From(
                                tag,
                                (Fraction)(tagCount / (double)itemCnt));
                        });
                    }).ToArray();
                return agg;
            });

        this.DisplayModeChanges = itemListChanges.CountChanged
            .Select(count => count > 1 ? DisplayMode.Fraction : DisplayMode.Simple);
    }

    public void Add(Tag tag)
    {
        _itemListChanges.Items.TrackedForEachAsync(
            item=>!item.Tags.Items.Contains(tag),
            item=>item.Tags.Add(tag),
            $"Adding Tag '{tag}' to {_itemListChanges.Count} Items",
            $"The tag has been added",
            $"The Tag '{tag}' has been added to {_itemListChanges.Count} items");
    }

    public void Remove(Tag tag)
    {
        _itemListChanges.Items.TrackedForEachAsync(
            item=>item.Tags.Items.Contains(tag),
            item=>item.Tags.Remove(tag),
            $"Removing Tag '{tag}' from {_itemListChanges.Count} Items",
            $"The tag has been replaced",
            $"The Tag '{tag}' has been removed from {_itemListChanges.Count} items");
    }

    public void Replace(Tag oldTag, Tag newTag)
    {
        _itemListChanges.Items.TrackedForEachAsync(
            item=>item.Tags.Items.Contains(oldTag),
                item=>item.Tags.Replace(oldTag, newTag),
                $"Replacing Tag '{oldTag}' with '{newTag}' on {_itemListChanges.Count} Items",
                $"The tag has been replaced",
                $"The Tag '{oldTag}' has replaced by '{newTag}' on {_itemListChanges.Count} items");
    }
    
    public IObservable<IEnumerable<FractionTag>> Tags { get; }
    public IObservable<DisplayMode> DisplayModeChanges { get; }

    public void Dispose()
    {
    }
}