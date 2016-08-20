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

        public static bool IsTrueNull(this Object obj)
        {
            return (object)obj == null;
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            if (obj == null) return;
            foreach (var t in obj.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = layer;
            }
        }

        public static bool LiesOnSegment(this Vector2 point, Vector2 a, Vector2 b, float tolerance = 0)
        {
            var aToB = b - a;
            bool onLine;
            if (aToB.x == 0)
                onLine = Mathf.Abs(a.x - point.x) <= tolerance;
            else
                onLine = Mathf.Abs((point.x - a.x) / aToB.x - (point.y - a.y) / aToB.y) <= tolerance;
            if (!onLine) return false;
            return point.x.IsBetweenOrEquals(a.x, b.x);
        }

        public static bool SegmentsIntersectOrCover(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            Vector2 v1 = end1 - start1;
            Vector2 v2 = end2 - start2;
            if (v1.normalized == v2.normalized)
                return
                    start1.LiesOnSegment(start2, end2, 0.0001f) ||
                    end1.LiesOnSegment(start2, end2, 0.0001f);

            var intersection = NonParalellLinesIntersection(start1, end1, start2, end2);
            bool result =
                intersection.x.IsBetweenOrEquals(start1.x, end1.x) &&
                intersection.x.IsBetweenOrEquals(start2.x, end2.x) &&
                intersection.y.IsBetweenOrEquals(start1.y, end1.y) &&
                intersection.y.IsBetweenOrEquals(start2.y, end2.y);
            return result;
        }

        public static Vector2 NonParalellLinesIntersection(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            Vector2 v1 = end1 - start1;
            Vector2 v2 = end2 - start2;

            bool line1Vertical = v1.x == 0;
            float cross1 = v1.Cross2(start1);
            float a1 = !line1Vertical ? v1.y / v1.x : 0;
            float b1 = !line1Vertical ? cross1 / v1.x : 0;
            float x1 = line1Vertical ? start1.x : 0;

            bool line2Vertical = v2.x == 0;
            float cross2 = v2.Cross2(start2);
            float a2 = !line2Vertical ? v2.y / v2.x : 0;
            float b2 = !line2Vertical ? cross2 / v2.x : 0;
            float x2 = line2Vertical ? start2.x : 0;
            
            // both cannot be vertical because they're not parelell, so there are 3 cases:
            if (line1Vertical)
                return new Vector2(x1, a2 * x1 + b2);
            else if (line2Vertical)
                return new Vector2(x2, a1 * x2 + b1);
            else
            {
                var x = (b1 - b2) / (a2 - a1);
                var y = a1 * x + b1;
                return new Vector2(x, y);
            }
        }

        public static float Cross2(this Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }

        public static bool ContainsWithBorder(this Rect rect, Vector2 point)
        {
            return rect.Contains(point) ||
                point.y == rect.yMax && rect.xMin <= point.x && point.x <= rect.xMax ||
                point.x == rect.xMax && rect.yMin <= point.y && point.y <= rect.yMax;
        }
    }
}