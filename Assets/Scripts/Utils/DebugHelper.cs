using System.Collections.Generic;
using System.Linq;

namespace MechWars.Utils
{
    public static class DebugHelper
    {
        public static string ToDebugMessage<T>(this IEnumerable<T> enumerable)
        {
            return string.Format("[ {0} ]", string.Join(", ", enumerable.Select(i => i == null ? "NULL" : i.ToString()).ToArray()));
        }
    }
}
