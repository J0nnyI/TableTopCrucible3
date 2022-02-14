using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;

namespace TableTopCrucible.Core.Helper;

public static class DynamicDataHelper
{
    #region filter

    public static IObservable<IChangeSet<TObject, TKey>> Filter<TObject, TKey>(
        this IObservable<IChangeSet<TObject, TKey>> source, Func<TObject, bool> filter,
        IObservable<Unit> reapplyFilter)
    {
        return source.Filter(reapplyFilter.Select(_ => filter));
    }

    #endregion

    #region sort

    private class Sorter<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _comparer;

        public Sorter(Func<T, T, int> comparer)
        {
            _comparer = comparer;
        }

        public int Compare([AllowNull] T x, [AllowNull] T y) => _comparer(x, y);
    }

    public static IObservable<ISortedChangeSet<TObject, TKey>> Sort<TObject, TKey>(
        this IObservable<IChangeSet<TObject, TKey>> source,
        IObservable<Func<TObject, TObject, int>> comparer)
    {
        return source.Sort(comparer.Select(comp => new Sorter<TObject>(comp)));
    }

    public static IObservable<IChangeSet<TObject>> Sort<TObject, TKey>(
        this IObservable<IChangeSet<TObject>> source,
        IObservable<Func<TObject, TObject, int>> comparer)
    {
        return source.Sort(comparer.Select(comp => new Sorter<TObject>(comp)));
    }


    public static IObservable<ISortedChangeSet<TObject, TKey>> Sort<TObject, TKey>(
        this IObservable<IChangeSet<TObject, TKey>> source, Func<TObject, TObject, int> comparer) =>
        source.Sort(Observable.Return(new Sorter<TObject>(comparer)));

    public static IObservable<IChangeSet<TObject>> Sort<TObject>(this IObservable<IChangeSet<TObject>> source,
        Func<TObject, TObject, int> comparer) =>
        source.Sort(Observable.Return(new Sorter<TObject>(comparer)));


    public static IObservable<ISortedChangeSet<TObject, TKey>> Sort<TObject, TKey>(
        this IObservable<IChangeSet<TObject, TKey>> source,
        SortDirection sortDirection = SortDirection.Ascending) where TObject : IComparable
    {
        return source.Sort((a, b) => a.CompareTo(b) * (sortDirection == SortDirection.Descending
            ? -1
            : 1));
    }

    public static IObservable<IChangeSet<TObject>> Sort<TObject>(this IObservable<IChangeSet<TObject>> source,
        SortDirection sortDirection = SortDirection.Ascending) where TObject : IComparable
    {
        return source.Sort((a, b) => a.CompareTo(b) * (sortDirection == SortDirection.Descending
            ? -1
            : 1));
    }


    public static IObservable<ISortedChangeSet<TObject, TKey>> Sort<TObject, TKey>(
        this IObservable<IChangeSet<TObject, TKey>> source, Func<TObject, IComparable> textRetriever,
        SortDirection sortDirection = SortDirection.Ascending)
    {
        return source.Sort((a, b) => textRetriever(a).CompareTo(textRetriever(b)) *
                                     (sortDirection == SortDirection.Descending
                                         ? -1
                                         : 1));
    }

    public static IObservable<IChangeSet<TObject>> Sort<TObject>(this IObservable<IChangeSet<TObject>> source,
        Func<TObject, IComparable> textRetriever, SortDirection sortDirection = SortDirection.Ascending)
    {
        return source.Sort((a, b) => textRetriever(a).CompareTo(textRetriever(b)) *
                                     (sortDirection == SortDirection.Descending
                                         ? -1
                                         : 1));
    }

    #endregion
}