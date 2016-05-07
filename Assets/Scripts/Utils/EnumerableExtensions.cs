using System.Collections.Generic;
using System.Linq;

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

        public static bool None<T>(this IEnumerable<T> collection, System.Func<T, bool> predicate)
        {
            return !collection.Any(predicate);
        }

        public static void RemoveFirst<T>(this IList<T> list)
        {
            list.RemoveAt(0);
        }

        public static void RemoveFirst<T>(this IList<T> list, System.Func<T, bool> predicate)
        {
            int i = 0;
            for (i = 0; i < list.Count; i++)
                if (predicate(list[i]))
                    break;
            list.RemoveAt(i);
        }
    }
}
