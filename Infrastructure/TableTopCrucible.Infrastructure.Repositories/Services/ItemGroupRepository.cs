using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Services;

public interface IItemGroupRepository : IRepository<ItemGroupId, ItemGroup>
{
}

internal class ItemGroupRepository : RepositoryBase<ItemGroupId, ItemGroup>, IItemGroupRepository
{
    public ItemGroupRepository(IStorageController storageController) : base(storageController,
        storageController.ItemGroups)
    {
    }
}