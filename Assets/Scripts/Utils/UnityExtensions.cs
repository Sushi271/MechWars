using UnityEngine;

namespace MechWars.Utils
{
    public static class UnityExtensions
    {
        public static Vector2 VX(this Vector2 v) { return new Vector2(v.x, 0); }
        public static Vector2 VY(this Vector2 v) { return new Vector2(0, v.y); }

        public static IVector2 Floor(this Vector2 v) { return (IVector2)v; }
        public static IVector2 Round(this Vector2 v) { return (IVector2)new Vector2(v.x + 0.5f, v.y + 0.5f); }
        public static IVector2 Ceiling(this Vector2 v) { return (IVector2)new Vector2(v.x + 1, v.y + 1); }
        
        public static bool IntersectsStrictly(this Bounds thisBounds, Bounds other)
        {
            return
                Mathf.Abs(thisBounds.center.x - other.center.x) < thisBounds.extents.x + other.extents.x &&
                Mathf.Abs(thisBounds.center.y - other.center.y) < thisBounds.extents.y + other.extents.y &&
                Mathf.Abs(thisBounds.center.z - other.center.z) < thisBounds.extents.z + other.extents.z;
        }

        public static GameObject CreateFieldCollider()
        {
            var fieldCollider = GameObject.Instantiate<GameObject>(Globals.Instance.fieldCollider);
            fieldCollider.name = "_FieldCollider";
            return fieldCollider;
        }
    }
}