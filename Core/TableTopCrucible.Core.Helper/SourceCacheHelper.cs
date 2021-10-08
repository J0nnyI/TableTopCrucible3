using System;
using System.Reactive.Linq;

namespace TableTopCrucible.Core.Helper
{
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
    }
}