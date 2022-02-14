using System.Collections.Generic;
using System.Linq;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services;

[Singleton]
public interface IItemRepository : IRepository<ItemId, Item>
{
    IEnumerable<Item> ByModelHash(FileHashKey hashKey);
}

internal class ItemRepository
    : RepositoryBase<ItemId, Item>,
        IItemRepository
{
    public ItemRepository(IStorageController storageController)
        : base(storageController, storageController.Items)
    {
    }

    public IEnumerable<Item> ByModelHash(FileHashKey hashKey)
        => Data.Items.Where(x => x.FileKey3d == hashKey);
}