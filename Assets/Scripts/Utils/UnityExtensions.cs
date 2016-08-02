using System.Collections.Generic;
using UnityEngine;

namespace MechWars.Utils
{
    public static class UnityExtensions
    {
        static readonly IVector2[] neighbourDeltas =
        {
            new IVector2(-1, -1),
            new IVector2(-1, 0),
            new IVector2(-1, 1),
            new IVector2(0, -1),
            new IVector2(0, 1),
            new IVector2(1, -1),
            new IVector2(1, 0),
            new IVector2(1, 1)
        };
        public static IEnumerable<IVector2> NeighbourDeltas { get { return neighbourDeltas; } }

        public static Vector2 VX(this Vector2 v) { return new Vector2(v.x, 0); }
        public static Vector2 VY(this Vector2 v) { return new Vector2(0, v.y); }

        public static IVector2 Floor(this Vector2 v) { return new IVector2((int)System.Math.Floor(v.x), (int)System.Math.Floor(v.y)); }
        public static IVector2 Round(this Vector2 v) { return (IVector2)new Vector2(v.x + 0.5f, v.y + 0.5f); }
        public static IVector2 Ceiling(this Vector2 v) { return (IVector2)new Vector2(v.x + 1, v.y + 1); }

        public static Vector2 AsHorizontalVector2(this Vector3 v) { return new Vector2(v.x, v.z); }
        public static Vector3 AsHorizontalVector3(this Vector2 v) { return new Vector3(v.x, 0, v.y); }
        public static Vector3 AsHorizontalVector3(this IVector2 v) { return new Vector3(v.X, 0, v.Y); }

        public static IVector2 Sign(this IVector2 v) { return new IVector2(System.Math.Sign(v.X), System.Math.Sign(v.Y)); }
        public static bool IsNeighbourTo(this IVector2 u, IVector2 v)
        {
            var delta = v - u;
            var absDX = Mathf.Abs(delta.X);
            var absDY = Mathf.Abs(delta.Y);
            return absDX <= 1 && absDY <= 1 && (absDX != 0 || absDY != 0);
        }

        public static bool IsInteger(this Vector2 v) { return v.x == (int)v.x && v.y == (int)v.y; }

        static float AngleFromTo(Vector2 from, Vector2 to)
        {
            var angleValue = Vector2.Angle(from, to);
            var angleSign = Mathf.Sign(from.x * to.y - from.y * to.x);
            return angleValue * angleSign;
        }

        public static float AngleFromToXZ(Vector2 from, Vector2 to)
        {
            // minus is because of cross product (x1*y2 - x2*y1 versus x2*z1 - x1*z2)
            return -AngleFromTo(from, to);
        }

        public static bool IntersectsStrictly(this Bounds thisBounds, Bounds other)
        {
            return
                Mathf.Abs(thisBounds.center.x - other.center.x) < thisBounds.extents.x + other.extents.x &&
                Mathf.Abs(thisBounds.center.y - other.center.y) < thisBounds.extents.y + other.extents.y &&
                Mathf.Abs(thisBounds.center.z - other.center.z) < thisBounds.extents.z + other.extents.z;
        }

        public static string TransformToString(this Transform transform)
        {
            return string.Format("P: {0}\nR: {1}\nS: {2}", transform.localPosition, transform.localRotation, transform.localScale);
        }

        public static bool IsTrueNull(this UnityEngine.Object obj)
        {
            return (object)obj == null;
        }
    }
}