﻿using System;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.EntityIds
{
    public interface IDataId
    {
        Guid Guid { get; init; }
    }

    public abstract class DataIdBase<TThis> : IdBase<TThis>, IDataId
        where TThis : IdBase<TThis>, new()
    {
        public Guid Guid
        {
            get => Value;
            init => Value = value;
        }
    }
}