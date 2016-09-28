using System;

namespace MechWars.Utils
{
    public static class Random2
    {
        public static int Sign()
        {
            return Math.Sign(new Random().Next(2) - 0.5);
        }
    }
}
