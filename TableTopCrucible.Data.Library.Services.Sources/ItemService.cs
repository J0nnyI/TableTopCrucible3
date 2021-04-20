using DynamicData;

using System;
using System.Collections.Generic;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.ValueTypes.IDs;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    [Singleton(typeof(ItemService))]
    public interface IItemService
    {
        void Edit(Action<ISourceUpdater<Item, ItemId>> updateAction);
        IObservableCache<Item, ItemId> GetCache();
        void AddOrUpdate(Item item);
        void AddOrUpdate(IEnumerable<Item> items);
    }
    internal class ItemService : IItemService
    {
        private SourceCache<Item, ItemId> itemCache = new SourceCache<Item, ItemId>(i => i.Id);
        public void Edit(Action<ISourceUpdater<Item, ItemId>> updateAction) => itemCache.Edit(updateAction);
        public IObservableCache<Item, ItemId> GetCache() => itemCache;
        public void AddOrUpdate(IEnumerable<Item> items)
            => itemCache.AddOrUpdate(items);
        public void AddOrUpdate(Item item)
            => itemCache.AddOrUpdate(item);
    }
}
