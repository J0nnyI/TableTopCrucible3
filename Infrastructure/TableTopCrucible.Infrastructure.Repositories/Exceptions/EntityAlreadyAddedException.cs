using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Repositories.Exceptions
{
    public class EntityAlreadyAddedException:Exception
    {
        public EntityAlreadyAddedException(Exception innerException):base("the entity has already been added",innerException)
        {
            
        }
    }

    public class EntityAlreadyAddedException<TId, TEntity> : EntityAlreadyAddedException
        where TId : IDataId
        where TEntity : class, IDataEntity<TId>, new()
    {
        public TEntity Entity { get; }

        public EntityAlreadyAddedException(Exception innerException, TEntity entity):base(innerException)
        {
            Entity = entity;
            this.Data.Add("entity",entity);
        }
    }
}
