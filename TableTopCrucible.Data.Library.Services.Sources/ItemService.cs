using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    [Singleton(typeof(IItemService))]
    public interface IItemService
    {

    }
    internal class ItemService:IItemService
    {
    }
}
