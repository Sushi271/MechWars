﻿using System.Collections.Generic;
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

        public static IVector2 Sign(this IVector2 v) { return new IVector2(System.Math.Sign(v.X), System.Math.Sign(v.Y)); }

        public static bool IntersectsStrictly(this Bounds thisBounds, Bounds other)
        {
            return
                Mathf.Abs(thisBounds.center.x - other.center.x) < thisBounds.extents.x + other.extents.x &&
                Mathf.Abs(thisBounds.center.y - other.center.y) < thisBounds.extents.y + other.extents.y &&
                Mathf.Abs(thisBounds.center.z - other.center.z) < thisBounds.extents.z + other.extents.z;
        }
    }
}