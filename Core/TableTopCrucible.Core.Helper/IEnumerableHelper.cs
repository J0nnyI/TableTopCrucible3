using System;
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
        public static bool None<T>(this IEnumerable<T> list, Func<T,bool> selector)
            => !list.Any(selector);
    }
}