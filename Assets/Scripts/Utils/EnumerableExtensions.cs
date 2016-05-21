using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.Utils
{
    public static class EnumerableExtensions
    {
        public static bool Empty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }
        
        public static bool Empty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.HasAtLeast(1);
        }
        
        public static bool HasAtLeast<T>(this IEnumerable<T> enumerable, int numberOfElements)
        {
            if (numberOfElements == 0) return true;

            int i = 0;
            foreach (var item in enumerable)
            {
                i++;
                if (i == numberOfElements) return true;
            }
            return false;
        }

        public static IEnumerable<T> AsEnumerable<T>(this T item, bool nullAsEmpty = false)
        {
            if (item == null && nullAsEmpty) yield break;
            yield return item;
        }

        public static T SelectMin<T, TResult>(this IEnumerable<T> enumerable, System.Func<T, TResult> selector)
            where TResult : System.IComparable
        {
            return enumerable.Aggregate((a, b) => selector(a).CompareTo(selector(b)) < 0 ? a : b);
        }

        public static T SelectMax<T, TResult>(this IEnumerable<T> enumerable, System.Func<T, TResult> selector)
            where TResult : System.IComparable
        {
            return enumerable.Aggregate((a, b) => selector(a).CompareTo(b) > 0 ? a : b);
        }

        public static IEnumerable<TResult> SelectDistinct<T, TResult>(this IEnumerable<T> enumerable, System.Func<T, TResult> selector)
        {
            return enumerable.Select(selector).Distinct();
        }

        public static bool None<T>(this IEnumerable<T> enumerable, System.Func<T, bool> predicate)
        {
            return !enumerable.Any(predicate);
        }
        
        public static FirstOrAnotherResult<T> FirstOrAnother<T>(this IEnumerable<T> enumerable,
            params System.Func<T, bool>[] predicates)
        {
            var firsts = new T[predicates.Length];
            int minFoundPredicate = predicates.Length;

            FirstOrAnotherResult<T> result = null;

            foreach (var item in enumerable)
            {
                for (int i = 0; i < minFoundPredicate; i++)
                {
                    if (predicates[i](item))
                    {
                        firsts[i] = item;
                        minFoundPredicate = i;
                        if (i == 0)
                        {
                            result = new FirstOrAnotherResult<T>(firsts[minFoundPredicate]);
                            break;
                        }
                    }
                }
                if (result != null) break;
            }

            if (result == null)
                if (minFoundPredicate == predicates.Length)
                    result = new FirstOrAnotherResult<T>();
                else result = new FirstOrAnotherResult<T>(firsts[minFoundPredicate]);

            return result;
        }

        public class FirstOrAnotherResult<T>
        {
            public bool Found { get; private set; }
            public T Result { get; private set; }

            public FirstOrAnotherResult()
            {
                Found = false;
                Result = default(T);
            }

            public FirstOrAnotherResult(T result)
            {
                Found = true;
                Result = result;
            }
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

        public static int RemoveWhereNot<T>(this HashSet<T> set, System.Func<T, bool> predicate)
        {
            return set.RemoveWhere(i => !predicate(i));
        }

        public static Vector2 Average<TSource>(this IEnumerable<TSource> source, System.Func<TSource, Vector2> selector)
        {
            var sum = Vector2.zero;
            int count = 0;
            foreach (var item in source)
            {
                sum += selector(item);
                count += 1;
            }
            return sum / count;
        }

        public static string ToDebugMessage<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.AllToString(i => i == null ? "NULL" : i.ToString());
        }

        public static string AllToString<T>(this IEnumerable<T> enumerable, System.Func<T, string> serializer)
        {
            return string.Format("[ {0} ]", string.Join(", ", enumerable.Select(i => serializer(i)).ToArray()));
        }
    }
}
