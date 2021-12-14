using System;
using System.Collections.Generic;
using System.Printing;
using System.Runtime.CompilerServices;

using Microsoft.VisualBasic.CompilerServices;

using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{

    public interface IDataEntity<TId>
        where TId : IDataId
    {
        TId Id { get; init; }
    }
    /// <summary>
    /// used for entities like item
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class DataEntity<TId> : ComplexDataType, IDataEntity<TId>
        where TId : IDataId, new()
    {
        public TId Id { get; init; }

        protected DataEntity()
        {
            Id = new TId { Guid = Guid.NewGuid() };
        }
    }
}