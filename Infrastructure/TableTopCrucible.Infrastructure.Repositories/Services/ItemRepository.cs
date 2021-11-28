using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IItemRepository : ISourceRepository<ItemId, Item, ItemDto> { }
    internal class ItemRepository : SourceRepositoryBase<ItemId, Item, ItemDto>, IItemRepository
    {
        public ItemRepository(IDatabase database) : base(database)
        {
        }
    }
}
