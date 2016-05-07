using UnityEngine;

namespace MechWars.Utils
{
    public struct IVector2
    {
        const int prime1 = 2147483647;
        const int prime2 = 16777619;

        public int X { get; private set; }
        public int Y { get; private set; }

        public float Length { get { return Mathf.Sqrt(X * X + Y * Y); } }
        public Vector2 Normalized { get { return this / Length; } }

        public IVector2(int x, int y)
            : this()
        {
            X = x;
            Y = y;
        }

        public float Dot(IVector2 v)
        {
            return X * v.X + Y * v.Y;
        }

        public float Cross(IVector2 v)
        {
            return X * v.Y - Y * v.X;
        }

        public static IVector2 operator +(IVector2 v)
        {
            return v;
        }

        public static IVector2 operator -(IVector2 v)
        {
            return new IVector2(-v.X, -v.Y);
        }
        
        public static IVector2 operator +(IVector2 v1, IVector2 v2)
        {
            return new IVector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static IVector2 operator -(IVector2 v1, IVector2 v2)
        {
            return v1 + (-v2);
        }

        public static IVector2 operator *(IVector2 v, int a)
        {
            return new IVector2(v.X * a, v.Y * a);
        }

        public static IVector2 operator *(int a, IVector2 v)
        {
            return v * a;
        }

        public static Vector2 operator *(IVector2 v, float a)
        {
            return new Vector2(v.X * a, v.Y * a);
        }

        public static Vector2 operator *(float a, IVector2 v)
        {
            return v * a;
        }

        public static Vector2 operator /(IVector2 v, float a)
        {
            return v * (1 / a);
        }

        public static float operator *(IVector2 v1, IVector2 v2)
        {
            return v1.Dot(v2);
        }

        public static implicit operator Vector2(IVector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static explicit operator IVector2(Vector2 v)
        {
            return new IVector2((int)v.x, (int)v.y);
        }

        public static bool operator ==(IVector2 v1, IVector2 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(IVector2 v1, IVector2 v2)
        {
            return !(v1 == v2);
        }

        public static float Distance(IVector2 v1, IVector2 v2)
        {
            return (v2 - v1).Length;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = prime1;
                hash = (hash * prime2) ^ X.GetHashCode();
                hash = (hash * prime2) ^ Y.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IVector2)) return false;
            var v = (IVector2)obj;
            return v.X == X && v.Y == Y;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}