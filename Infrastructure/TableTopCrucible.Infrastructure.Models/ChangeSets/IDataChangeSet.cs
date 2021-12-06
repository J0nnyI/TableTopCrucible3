using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;

namespace TableTopCrucible.Infrastructure.Models.ChangeSets
{
    public interface IDataChangeSet<TId, out TModel, out TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId, TModel>
        where TModel : class, IDataModel<TId>
    {
        TEntity ToEntity();
        TModel ToModel();
    }
}