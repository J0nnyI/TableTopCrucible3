using DynamicData;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    [Singleton(typeof(IItemService))]
    public interface IItemService
    {

    }
    internal class ItemService : IItemService
    {
        private SourceCache<Item, ItemId> itemCache = new SourceCache<Item, ItemId>(i=>i.Id);

    }
}
