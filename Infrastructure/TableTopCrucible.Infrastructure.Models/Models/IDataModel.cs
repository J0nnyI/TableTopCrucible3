using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Models
{
    public interface IDataModel<out TId, TThis, out TEntity>
        where TId : IDataId
        where TEntity : IDataEntity<TId, TThis, TEntity>
        where TThis : IDataModel<TId, TThis, TEntity>
    {
        TId Id { get; }
        TEntity Entity { get; }
    }
}