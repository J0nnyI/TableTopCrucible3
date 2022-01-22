using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using ReactiveUI;

using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public interface IDataEntity<TId>
        where TId : IDataId
    {
        TId Id { get; }
        Guid Guid { get; }
    }

    /// <summary>
    ///     used for entities like item
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class DataEntity<TId> : ReactiveObject, IDataEntity<TId>
        where TId : IDataId, new()
    {
        private TId _id;

        protected DataEntity()
        {
            Id = new TId { Guid = Guid.NewGuid() };
        }

        [JsonProperty]
        public Guid Guid
        {
            get => Id.Guid;
            set => Id = new TId { Guid = value };
        }
        public TId Id
        {
            get => _id;
            set
            {
                if (value is null)
                    throw new NullReferenceException(nameof(Id));
                _id = value;
            }
        }

        public void SetValue<TValue>(ref TValue field, TValue value,
            params string[] names)
        {
            if (field?.Equals(value) == true)
                return;

            field = value;
            foreach (var name in names)
                this.RaisePropertyChanged(name);
        }
        public void SetRequiredValue<TValue>(ref TValue field, TValue value,
            params string[] names)
        {
            if (value is null)
                throw new NullReferenceException(string.Join(", ", names));
            SetValue(ref field, value, names);
        }
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
            => SetRequiredValue(ref field, value, new[] { name });
    }
}