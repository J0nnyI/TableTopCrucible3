
using System;
using System.Linq;
using System.Reactive.Linq;

namespace TableTopCrucible.Core.Helper
{
    public static class SourceCacheHelper
    {

        public static IObservable<Func<Tcache, bool>> ToFilter
            <Tcache, Tobservable>
            (
            this IObservable<Tobservable> observable,
            Func<Tcache, Tobservable, bool> filter
            )
        {
            return observable.Select(value =>
                new Func<Tcache, bool>(item => filter(item, value))
            );
        }

    }
}
