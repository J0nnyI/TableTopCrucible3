using System.Collections.Generic;
using System.Linq;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services
{
    [Singleton]
    public interface IItemRepository : IRepository<ItemId, Item>
    {
        public IEnumerable<Item> this[FileHashKey hashKey] { get; }
    }

    internal class ItemRepository
        : RepositoryBase<ItemId, Item>,
            IItemRepository
    {
        public ItemRepository(IDatabaseAccessor database)
            : base(database, database.Items)
        {
        }
        public IEnumerable<Item> this[FileHashKey hashKey]
            => hashKey is null
                ? Enumerable.Empty<Item>()
                : this.Data.Get($"item by HashKey {hashKey}",
                    data => data
                        .Where(item => item.FileKey3dRaw == hashKey.Value));

        public override string TableName => ItemConfiguration.TableName;
    }
}