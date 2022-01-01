﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using ReactiveUI;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public interface IDataEntity<TId>
        where TId : IDataId
    {
        TId Id { get; init; }
    }

    /// <summary>
    ///     used for entities like item
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class DataEntity<TId> : ReactiveObject, IDataEntity<TId>
        where TId : IDataId, new()
    {
        private readonly TId _id;

        protected DataEntity()
        {
            Id = new TId { Guid = Guid.NewGuid() };
        }

        public Guid Guid {
            get=> Id.Guid;
            init => Id = new TId { Guid = value };
        }
        public TId Id
        {
            get => _id;
            init
            {
                if (value is null)
                    throw new NullReferenceException(nameof(Id));
                _id = value;
            }
        }

        protected abstract IEnumerable<object> getAtomicValues();

        public override bool Equals(object obj)
            => obj.GetType() == GetType()
               && obj is DataEntity<TId> other
               && Id.Equals(other.Id)
               && getAtomicValues().SequenceEqual(other.getAtomicValues());

        public override int GetHashCode()
            => getAtomicValues()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate(HashCode.Combine);

        /// <summary>
        ///     same as <see cref="IReactiveObjectExtensions.RaiseAndSetIfChanged{TObj,TRet}" /> but it throws an
        ///     <see cref="NullReferenceException" /> if the value is null
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public void SetRequiredValue<TValue>(ref TValue field, TValue value,
            [CallerMemberName] string name = "")
        {
            if (value is null)
                throw new NullReferenceException(name);
            this.RaiseAndSetIfChanged(ref field, value, name);
        }
    }
}