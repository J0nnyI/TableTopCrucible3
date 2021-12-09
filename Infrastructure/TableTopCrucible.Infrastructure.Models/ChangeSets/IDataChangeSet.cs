using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ReactiveUI;

using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Models.Models;
using TableTopCrucible.Infrastructure.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Models.ChangeSets
{
    public interface IDataChangeSet<out TId, TModel, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId, TModel, TEntity>, new()
        where TModel : class, IDataModel<TId, TModel, TEntity>
    {
        TId Id { get; }

        TEntity Entity { get; }
        TModel OriginalModel { get; }
        void ApplyChanges(out TModel newModel);
        IReadOnlyDictionary<string, PropertyName> UpdatedProperties { get; }
    }

    public abstract class DataChangeSet<TId, TModel, TEntity>
        : ReactiveObject,
            IDataChangeSet<TId, TModel, TEntity>
        where TId : IDataId
        where TEntity : class, IDataEntity<TId, TModel, TEntity>, new()
        where TModel : class, IDataModel<TId, TModel, TEntity>
    {
        public TId Id { get; init; }
        public abstract TEntity Entity { get; init; }
        public TModel OriginalModel { get; init; }
        public abstract void ApplyChanges(out TModel newModel);


        private readonly Dictionary<string, PropertyName> _updatedProperties = new();
        public IReadOnlyDictionary<string, PropertyName> UpdatedProperties { get; }
        protected DataChangeSet()
        {
            UpdatedProperties = new ReadOnlyDictionary<string, PropertyName>(_updatedProperties);

            this.PropertyChanged += (_, e) =>
                _updatedProperties.Add(e.PropertyName, PropertyName.From(e.PropertyName));
        }

    }

    public class PropertyMapper
        <TValue, TId, TModel, TEntity, TChangeSet>
        : ReactiveObject
        where TId : IDataId
        where TEntity : class, IDataEntity<TId, TModel, TEntity>, new()
        where TModel : class, IDataModel<TId, TModel, TEntity>
        where TChangeSet : class, IDataChangeSet<TId, TModel, TEntity>
    {
        public PropertyMapper(TChangeSet changeSet, string propertyName, Action<TValue> entityWriter)
        {

        }
        public void ApplyChanges()
        {

        }
    }
}