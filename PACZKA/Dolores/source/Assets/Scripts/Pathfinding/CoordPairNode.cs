using System;
using MechWars.Utils;

namespace MechWars.Pathfinding
{
    public class CoordPairNode<T> : IHeapNode<T>
        where T : IComparable<T>
    {
        public virtual T Key { get { return Distance; } }
        public int HeapIndex { get; set; }

        public T Distance { get; set; }

        public CoordPair CoordPair { get; private set; }

        public CoordPairNode(CoordPair coordPair)
        {
            Distance = default(T);
            CoordPair = coordPair;
        }
    }
}
