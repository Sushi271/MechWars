using System.Collections.Generic;
using System.Linq;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.Pathfinding
{
    public struct CoordPair
    {
        static IEnumerable<CoordPair> neighbourDeltas =
            from v in UnityExtensions.NeighbourDeltas
            select new CoordPair(v);

        public IVector2 Vector { get; private set; }
        public int X { get { return Vector.X; } }
        public int Y { get { return Vector.Y; } }
        
        public CoordPair(IVector2 v)
            : this()
        {
            Vector = v;
        }

        public CoordPair(int x, int y)
            : this(new IVector2(x, y))
        {
        }

        public static CoordPair operator +(CoordPair cp)
        {
            return cp;
        }

        public static CoordPair operator -(CoordPair cp)
        {
            return new CoordPair(-cp.Vector);
        }

        public static CoordPair operator +(CoordPair cp1, CoordPair cp2)
        {
            return new CoordPair(cp1.Vector + cp2.Vector);
        }

        public static CoordPair operator -(CoordPair cp1, CoordPair cp2)
        {
            return new CoordPair(cp1.Vector - cp2.Vector);
        }

        public static explicit operator CoordPair(IVector2 v)
        {
            return new CoordPair(v);
        }

        public static explicit operator IVector2(CoordPair cp)
        {
            return cp.Vector;
        }

        public static bool operator ==(CoordPair cp1, CoordPair cp2)
        {
            return cp1.Vector == cp2.Vector;
        }

        public static bool operator !=(CoordPair cp1, CoordPair cp2)
        {
            return cp1.Vector != cp2.Vector;
        }

        public static float Distance(CoordPair cp1, CoordPair cp2)
        {
            return IVector2.Distance(cp1.Vector, cp2.Vector);
        }

        public override int GetHashCode()
        {
            return (Globals.Map.Size * Y) + X;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is CoordPair)) return false;

            var other = (CoordPair)obj;
            return other == this;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }

        public IEnumerable<CoordPair> Neighbours
        {
            get
            {
                var _this = this;
                var map = Globals.Map;
                return
                    from nd in neighbourDeltas
                    let cp = _this + nd
                    where
                        0 <= cp.X && cp.X < map.Size &&
                        0 <= cp.Y && cp.Y < map.Size
                    select cp;
            }
        }
    }
}
