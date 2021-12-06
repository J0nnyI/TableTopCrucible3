using System;

using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public interface IDataEntity<TId, out TModel>
    where TModel : IDataModel<TId>
    where TId : IDataId
    {
        Guid Id { get; set; }
        TModel ToModel();
    }
}