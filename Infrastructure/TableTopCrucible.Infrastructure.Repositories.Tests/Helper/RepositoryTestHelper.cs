using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Infrastructure.Repositories.Tests.Helper
{
    public static class RepositoryTestHelper
    {
        public static void Clear<TId, TEntity>(this IRepository<TId, TEntity> repository)
            where TId : IDataId
            where TEntity : class, IDataEntity<TId>, new()
        => repository.RemoveRange(repository.Data);
    }
}
