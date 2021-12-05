using System;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public interface IDataEntity
    {
        Guid Id { get; }
    }
}