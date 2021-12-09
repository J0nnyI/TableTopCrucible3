using System;

using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public interface IDataEntity<TId, out TModel, TThis>
    where TModel : IDataModel<TId, TModel, TThis>
    where TThis : IDataEntity<TId, TModel, TThis>
    where TId : IDataId
    {
        Guid Id { get; set; }
        TModel ToModel();
    }
}