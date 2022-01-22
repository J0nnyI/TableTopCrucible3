using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models;

namespace TableTopCrucible.Infrastructure.Repositories.Helper
{
    public static class CollectionUpdateHelper
    {
        public static IObservable<IChangeSet<TEntity, TId>> ToObservableCache<TId, TEntity>(
            this IObservable<CollectionUpdate<TId, TEntity>> updateSource,
            CompositeDisposable disposeWith
        )
            where TId : IDataId
            where TEntity : class, IDataEntity<TId>, new()
        {
            return updateSource
                .Scan(
                    new SourceCache<TEntity, TId>(entity => entity.Id)
                        .DisposeWith(disposeWith),
                    (list, change) =>
                    {
                        switch (change.UpdateInfo.ChangeReason)
                        {
                            case EntityUpdateChangeReason.Add:
                                list.AddOrUpdate(change.UpdateInfo.UpdatedEntities.Values);
                                break;
                            case EntityUpdateChangeReason.Remove:
                                list.Remove(change.UpdateInfo.UpdatedEntities.Keys);
                                break;
                            case EntityUpdateChangeReason.Init:
                                ObservableCacheEx.AddOrUpdate<TEntity, TId>(list, Enumerable.AsEnumerable<TEntity>(change.Queryable));
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        return list;
                    })
                .Select(cache => cache.Connect())
                .Switch();
        }
    }
}