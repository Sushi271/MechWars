using System;
using System.Collections.Generic;
using System.Text;

namespace MechWars.Utils
{
    public static class DotNetExtensions
    {
        public static T Choice<T>(this Random random, List<T> list)
        {
            return list[random.Next(list.Count)];
        }

        public static StringBuilder AppendFormatLine(this StringBuilder sb, string format, params object[] args)
        {
            return sb.AppendFormat(format, args).AppendLine();
        }
    }
}
