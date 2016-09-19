using System;

namespace MechWars.Utils
{
    public static class DebugHelper
    {
        public static void BreakPoint(Func<bool> condition = null)
        {
            if (condition == null || condition())
            {
                int x = 5;
                int y = x;
            }
        }
    }
}
