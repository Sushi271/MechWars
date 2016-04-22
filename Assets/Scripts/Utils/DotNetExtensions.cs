using System;
using System.Collections.Generic;

namespace MechWars.Utils
{
    public static class DotNetExtensions
    {
        public static T Choice<T>(this Random random, List<T> list)
        {
            return list[random.Next(list.Count)];
        }
    }
}
