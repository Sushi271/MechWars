using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.Utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static T SelectMin<T, TResult>(this IEnumerable<T> collection, System.Func<T, TResult> selector)
            where TResult : System.IComparable
        {
            return collection.Aggregate((a, b) => selector(a).CompareTo(selector(b)) < 0 ? a : b);
        }

        public static T SelectMax<T, TResult>(this IEnumerable<T> collection, System.Func<T, TResult> selector)
            where TResult : System.IComparable
        {
            return collection.Aggregate((a, b) => selector(a).CompareTo(b) > 0 ? a : b);
        }
    }
}
