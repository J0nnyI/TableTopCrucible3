using DynamicData;
using TableTopCrucible.Core.Database;
using TableTopCrucible.Core.Database.Models;

namespace TableTopCrucible.Infrastructure.Repositories
{
    public interface ISourceRepository<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {
        public IConnectableCache<Tentity, Tid> Data { get; }
        public void Add(Tentity entityOrUpdate);
    }
    public abstract class SourceRepositoryBase<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {
        private readonly ITable<Tid, Tentity, Tdto> _table;
        public IConnectableCache<Tentity, Tid> Data => _table.Data;
        public void Add(Tentity entityOrUpdate) => _table.AddOrUpdate(entityOrUpdate);
        protected SourceRepositoryBase(IDatabase database)
        {
            this._table = database.GetTable<Tid, Tentity, Tdto>();
        }
    }
}