using System;
using DynamicData;

using System.Collections.Generic;

using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Infrastructure.Repositories
{
    public interface ISourceRepository<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {
        IObservableCache<Tentity, Tid> Data { get; }
        void AddOrUpdate(Tentity entity);
        void AddOrUpdate(IEnumerable<Tentity> entities);
        void Delete(Tid id);
        void Delete(IEnumerable<Tid> id);
        Tentity this[Tid id] { get; }
        void Edit(Action<ISourceUpdater<Tentity, Tid>> updateAction);
    }
    public abstract class SourceRepositoryBase<Tid, Tentity, Tdto>
        : ISourceRepository<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {
        private readonly ITable<Tid, Tentity, Tdto> _table;
        public IObservableCache<Tentity, Tid> Data => _table.Data;
        public void Edit(Action<ISourceUpdater<Tentity, Tid>> updateAction)
            => _table.Edit(updateAction);

        public void AddOrUpdate(Tentity entity) => _table.AddOrUpdate(entity);
        public void AddOrUpdate(IEnumerable<Tentity> entities) => _table.AddOrUpdate(entities);

        public void Delete(IEnumerable<Tid> ids)
            => _table.Remove(ids);

        public Tentity this[Tid id] 
            => Data.Lookup(id).ToValue();

        public void Delete(Tid id)
            => _table.Remove(id);

        protected SourceRepositoryBase(IDatabase database)
        {
            this._table = database.GetTable<Tid, Tentity, Tdto>();
        }
    }
}