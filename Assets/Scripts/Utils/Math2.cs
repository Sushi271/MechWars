using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechWars.Utils
{
    public static class Math2
    {
        public static int AbsMin(int number1, int number2)
        {
            return Math.Abs(number1) <= Math.Abs(number2) ? number1 : number2;
        }

        public static float AbsMin(float number1, float number2)
        {
            return Math.Abs(number1) <= Math.Abs(number2) ? number1 : number2;
        }
    }
}
