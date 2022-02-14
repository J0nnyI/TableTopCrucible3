using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using TableTopCrucible.Core.Helper.Exceptions;

namespace TableTopCrucible.Core.Helper;

public static class SourceCacheHelper
{
    public static IObservable<Func<TCache, bool>> ToFilter
        <TCache, TObservable>
        (
            this IObservable<TObservable> observable,
            Func<TCache, TObservable, bool> filter
        )
    {
        return observable.Select(value =>
            new Func<TCache, bool>(item => filter(item, value))
        );
    }

    /// <summary>
    ///     adds the given objects to the cache<br />
    ///     throws an exception if a given key is already taken or added twice
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <param name="cache"></param>
    /// <param name="objects"></param>
    /// <exception cref="UniqueConstraintFailedException{TId,TObject}">
    ///     thrown when multiple objects with the same ID are added
    ///     or the id is already taken
    /// </exception>
    public static void Add<TObject, TId>(this ISourceCache<TObject, TId> cache, params TObject[] objects)
    {
        cache.Edit(updater =>
        {
            var alreadyAddedObjects =
                objects
                    .Select(x =>
                        new
                        {
                            oldObject = updater.Lookup(cache.KeySelector(x)),
                            newObject = x
                        })
                    .Where(x => x.oldObject.HasValue)
                    .Select(x => new
                    {
                        oldObject = x.oldObject.Value,
                        x.newObject
                    })
                    .ToArray();


            if (alreadyAddedObjects.Any())
                throw new UniqueConstraintFailedException<TId, TObject>(
                    alreadyAddedObjects
                        .Select(x =>
                            new ExceptionObjectInfo<TObject, TId>
                            {
                                Id = cache.KeySelector(x.oldObject),
                                OldObject = x.oldObject,
                                NewObjects = objects.Where(y =>
                                    cache.KeySelector(y).Equals(cache.KeySelector(x.oldObject)))
                            }));

            var duplicateAdds =
                objects
                    .GroupBy(cache.KeySelector)
                    .Where(g => g.Count() > 2)
                    .ToArray();

            if (duplicateAdds.Any())
                throw new UniqueConstraintFailedException<TId, TObject>(
                    duplicateAdds
                        .Select(x =>
                            new ExceptionObjectInfo<TObject, TId>
                            {
                                Id = x.Key,
                                NewObjects = x
                            }));

            cache.AddOrUpdate(objects);
        });
    }

    public static IEnumerable<TObject> RemoveItemsWhere<TObject, TId>(
        this ISourceCache<TObject, TId> cache,
        Func<TObject, bool> selector)
    {
        IEnumerable<TObject> items = null;
        cache.Edit(updater =>
            {
                items = updater.Items.Where(selector);
                updater.Remove(items);
            }
        );
        return items;
    }

    public static IObservable<TObject> WatchFirstOrDefault<TObject, TKey>(
        this IObservable<IChangeSet<TObject, TKey>> cache)
        => cache.ToCollection().Select(col => col.FirstOrDefault());

    public static void RemoveWhere<TObject>(this ISourceList<TObject> list, Func<TObject, bool> selector)
        => list.Edit(updater => updater.RemoveMany(updater.Where(selector)));

    public static IEnumerable<TObject> RemoveWhere<TObject, TKey>(this ICacheUpdater<TObject, TKey> updater,
        Func<TObject, bool> selector)
    {
        var items = updater
            .KeyValues
            .Where(kv => selector(kv.Value))
            .ToArray();

        updater.Remove(items.Select(kv => kv.Key));
        return items.Select(kv => kv.Value);
    }

    public static IDisposable SynchronizeWith<T>(this IObservableList<T> src, ISourceList<T> target,
        Action<T> srcAddAction, Action<T> srcRemoveAction)
    {
        CompositeDisposable _disposables = new();
        src.Connect()
            .OnItemAdded(item =>
            {
                if (!target.Items.Contains(item))
                    target.Add(item);
            })
            .OnItemRemoved(item =>
            {
                if (target.Items.Contains(item))
                    target.Remove(item);
            }).Subscribe()
            .DisposeWith(_disposables);
        target.Connect()
            .OnItemAdded(item =>
            {
                if (!src.Items.Contains(item))
                    srcAddAction(item);
            })
            .OnItemRemoved(item =>
            {
                if (src.Items.Contains(item))
                    srcRemoveAction(item);
            }).Subscribe()
            .DisposeWith(_disposables);
        return _disposables;
    }
}