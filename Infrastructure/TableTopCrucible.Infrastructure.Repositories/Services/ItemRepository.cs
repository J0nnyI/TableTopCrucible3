using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IItemRepository : IRepository<ItemId, ItemEntity>
    {
    }

    internal class ItemRepository
        : RepositoryBase<ItemId, ItemEntity>,
            IItemRepository
    {
        public ItemRepository(IDatabaseAccessor database)
            : base(database, database.Items)
        {
        }
    }
}