namespace MechWars.Utils
{
    public static class Math2
    {
        public static int AbsMin(int number1, int number2)
        {
            return System.Math.Abs(number1) <= System.Math.Abs(number2) ? number1 : number2;
        }

        public static float AbsMin(float number1, float number2)
        {
            return System.Math.Abs(number1) <= System.Math.Abs(number2) ? number1 : number2;
        }

        public static float NormalizeAngle(this float number)
        {
            return ((((number + 180) % 360) + 360) % 360) - 180;
        }

        public static bool IsPowerOfTwo(this int x)
        {
            return (x & (x - 1)) == 0;
        }

        public static float Reciproc(this float x)
        {
            return 1 / x;
        }

        public static float Reciproc(this int x)
        {
            return ((float)x).Reciproc();
        }

        public static bool IsBetween(this System.IComparable number, System.IComparable a, System.IComparable b)
        {
            int comparedToA = number.CompareTo(a);
            int comparedToB = number.CompareTo(b);
            return
                0 < comparedToA && comparedToB < 0 ||
                0 < comparedToB && comparedToA < 0;
        }

        public static bool IsBetweenOrEquals(this System.IComparable number, System.IComparable a, System.IComparable b)
        {
            int comparedToA = number.CompareTo(a);
            int comparedToB = number.CompareTo(b);
            return
                0 <= comparedToA && comparedToB <= 0 ||
                0 <= comparedToB && comparedToA <= 0;
        }
    }
}
