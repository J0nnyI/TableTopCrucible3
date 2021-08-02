using DynamicData;

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using TableTopCrucible.Core.BaseUtils;
using TableTopCrucible.Core.DataAccess;
using TableTopCrucible.Core.DataAccess.Models;
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.Models.DataSource;
using TableTopCrucible.Data.Library.Models.DataTransfer;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    [Singleton(typeof(ItemService))]
    public interface IItemService
    {
        IObservableCache<Item, ItemId> GetCache();
        void AddOrUpdate(Item item);
        void AddOrUpdate(IEnumerable<Item> items);
    }
    internal class ItemService : DisposableReactiveObject, IItemService
    {
        private readonly IDatabase _database;
        protected ITable<ItemId, Item, ItemDto> Table => _database.GetTable<ItemId, Item, ItemDto>();
        
        public ItemService(IDatabase database)
        {
            _database = database;
        }
        public IObservableCache<Item, ItemId> GetCache() => Table.Data;
        public void AddOrUpdate(IEnumerable<Item> items)
            => Table.AddOrUpdate(items);
        public void AddOrUpdate(Item item)
            => Table.AddOrUpdate(item);
    }
}
