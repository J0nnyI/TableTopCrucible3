﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using DynamicData;

namespace TableTopCrucible.Core.Helper
{
    public static class IEnumerableHelper
    {
        /**
         * {1,2,3,4,5} + (2,4) ==> {2,3,4}
         * {1,2,3,4,5} + (4,2) ==> {2,3,4}
         */
        public static IEnumerable<T> Subsection<T>(this IEnumerable<T> list, T elementA, T elementB)
        {
            var indexA = list.IndexOf(elementA);
            var indexB = list.IndexOf(elementB);
            var first = indexA < indexB
                ? indexA
                : indexB;
            var last = indexA > indexB
                ? indexA
                : indexB;
            return list.Skip(first).Take(last - first + 1);
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

        public static bool ContainsAll<T>(this IEnumerable<T> list, IEnumerable<T> compare)
            => compare.All(list.Contains);
        public static bool ContainsAny<T>(this IEnumerable<T> list, IEnumerable<T> compare)
            => compare.Any(list.Contains);


        /// returns the removed items
        public static IEnumerable<TObject> RemoveWhere<TObject>(this IList<TObject> list, Func<TObject, bool> selector)
        {
            var items = list.Where(selector).ToArray();
            list.Remove(items);
            return items;
        }
    }
}