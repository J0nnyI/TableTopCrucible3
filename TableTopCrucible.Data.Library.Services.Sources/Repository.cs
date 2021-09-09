using System;
using System.Collections.Generic;
using System.Text;
using TableTopCrucible.Core.DataAccess;
using TableTopCrucible.Core.DataAccess.Models;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    public interface IRepository<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {

    }
    internal class Repository<Tid, Tentity, Tdto> : IRepository<Tid, Tentity, Tdto>
        where Tid : IEntityId
        where Tentity : IEntity<Tid>
        where Tdto : IEntityDto<Tid, Tentity>
    {
        private readonly ITable<Tid, Tentity, Tdto> table;

        public Repository(IDatabase database)
        {
            this.table = database.GetTable<Tid, Tentity, Tdto>();
        }
    }
}
