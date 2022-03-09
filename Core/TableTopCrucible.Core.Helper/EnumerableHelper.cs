using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media.TextFormatting;
using DynamicData;

namespace TableTopCrucible.Core.Helper;

public static class EnumerableHelper
{
    /**
         * {1,2,3,4,5} + (2,4) ==> {2,3,4}
         * {1,2,3,4,5} + (4,2) ==> {2,3,4}
         */
    public static IEnumerable<T> Subsection<T>(this IEnumerable<T> list, T elementA, T elementB)
    {
        var enumeratedList = list.ToArray();
        var indexA = enumeratedList.IndexOf(elementA);
        var indexB = enumeratedList.IndexOf(elementB);
        var first = indexA < indexB
            ? indexA
            : indexB;
        var last = indexA > indexB
            ? indexA
            : indexB;
        return enumeratedList.Skip(first).Take(last - first + 1);
    }

    public static StringCollection ToStringCollection(this IEnumerable<string> list)
    {
        var fileList = new StringCollection();
        fileList.AddRange(list.ToArray());
        return fileList;
    }

    public static bool None<T>(this IEnumerable<T> list)
        => !list.Any();

    public static bool None<T>(this IEnumerable<T> list, Func<T, bool> selector)
        => !list.Any(selector);

    //returns null if the collection is empty or has more than one element
    public static T OnlyOrDefault<T>(this IEnumerable<T> list)
    {
        var arr = list.ToArray();
        return arr.Count() == 1
            ? arr.First()
            : default;
    }

    public static bool ContainsNot<T>(this IEnumerable<T> list, T compare)
        => !list.Contains(compare);
    /// <summary>
    /// list=[1,2,3]<br/>
    /// compare=[2,3]<br/>
    /// returns: true<br/>
    /// <br/>
    /// list=[1,2,3]<br/>
    /// compare=[2,3,<b>4</b>]<br/>
    /// returns: false
    /// </summary>
    /// <param name="list"></param>
    /// <param name="compare"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ContainsAll<T>(this IEnumerable<T> list, IEnumerable<T> compare)
        => compare.All(list.Contains);

    /// <summary>
    /// list=[1,2,3]<br/>
    /// compare=[4,5]<br/>
    /// returns: true<br/>
    /// <br/>
    /// list=[1,2,<b>3</b>]<br/>
    /// compare=[<b>3</b>,4,5]<br/>
    /// returns: false
    /// </summary>
    /// <param name="list"></param>
    /// <param name="compare"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ContainsNone<T>(this IEnumerable<T> list, IEnumerable<T> compare)
        => !list.ContainsAll(compare);

    /// <summary>
    /// list=[1,2,3]<br/>
    /// compare=[4,5]<br/>
    /// returns: false<br/>
    /// <br/>
    /// list=[1,2,<b>3</b>]<br/>
    /// compare=[<b>3</b>,4,5]<br/>
    /// returns: true
    /// </summary>
    /// <param name="list"></param>
    /// <param name="compare"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ContainsAny<T>(this IEnumerable<T> list, IEnumerable<T> compare)
        => compare.Any(list.Contains);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="selector"></param>
    /// <typeparam name="TObject"></typeparam>
    /// <returns>the removed items</returns>
    public static IEnumerable<TObject> RemoveWhere<TObject>(this IList<TObject> list, Func<TObject, bool> selector)
    {
        var items = list.Where(selector).ToArray();
        list.Remove(items);
        return items;
    }
    private class OrderHelper<T>:IComparer<T>
    {
        private readonly Func<T?, T?, int> _comparer;

        public OrderHelper(Func<T?,T?,int> comparer)
            => _comparer = comparer;

        public int Compare(T? x, T? y) => _comparer(x, y);
    }
    public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> list,Func<TSource,TKey> keySelector,  Func<TKey, TKey, int> sorter)
        => list.OrderBy(keySelector,new OrderHelper<TKey>(sorter));
    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list,  Func<T, T, int> sorter)
        => list.OrderBy(x=>x,sorter);
}