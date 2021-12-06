using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.ChangeSets;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IItemRepository:IRepository<ItemId, ItemModel, ItemEntity, ItemChangeSet> { }
    internal class ItemRepository 
        :RepositoryBase<ItemId,  ItemModel, ItemEntity, ItemChangeSet>,
        IItemRepository
    {
        public ItemRepository(IDatabaseAccessor database) 
            : base(database, database.Items)
        {
        }
    }
}
