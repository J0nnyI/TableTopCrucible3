using DynamicData;

using System;
using System.Collections.Generic;

using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.Database.Models;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories
{
    public interface ISourceRepository<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {
        public IConnectableCache<Tentity, Tid> DataChanges { get; }
        public IObservableCache<Tentity, Tid> Data { get; }
        public void AddOrUpdate(Tentity entity);
        public void Delete(Tid id);
    }
    public abstract class SourceRepositoryBase<Tid, Tentity, Tdto>
        : ISourceRepository<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {
        private readonly ITable<Tid, Tentity, Tdto> _table;
        public IConnectableCache<Tentity, Tid> DataChanges => _table.Data;
        public IObservableCache<Tentity, Tid> Data => _table.Data;
        public IObservable<IEnumerable<FileArchivePath>> TakenDirectoriesChanges { get; }

        public void AddOrUpdate(Tentity entity) => _table.AddOrUpdate(entity);
        public void Delete(Tid id)
            => _table.Remove(id);

        protected SourceRepositoryBase(IDatabase database)
        {
            this._table = database.GetTable<Tid, Tentity, Tdto>();
        }
    }
}