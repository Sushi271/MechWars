using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.Pathfinding
{
    public class Path : IEnumerable<WayPoint>
    {
        List<WayPoint> wayPoints;

        public int Count { get { return wayPoints.Count; } }
        public float Length { get; private set; }

        public WayPoint First { get { return Count == 0 ? null : wayPoints.Last(); } }
        public WayPoint Last { get { return Count == 0 ? null : wayPoints.First(); } }

        public bool Empty { get { return Count <= 1; } }
        
        public Path()
        {
            wayPoints = new List<WayPoint>();
        }

        public void Push(CoordPair coordPair)
        {
            var first = First;
            var wayPoint = new WayPoint(coordPair, first);
            wayPoints.Add(wayPoint);
            if (Count > 1)
            {
                var dist = CoordPair.Distance(first.Coords, wayPoint.Coords);
                Length += dist;
            }
        }

        public void Pop()
        {
            if (wayPoints.Count == 0)
                throw new System.Exception("Cannot pop - path is empty.");
            if (wayPoints.Count > 1)
            {
                var dist = CoordPair.Distance(wayPoints[1].Coords, Last.Coords);
                Length -= dist;
            }
            wayPoints.RemoveAt(Count - 1);
        }

        public IEnumerator<WayPoint> GetEnumerator()
        {
            return ((IEnumerable<WayPoint>)wayPoints).Reverse().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var strings =
                from wp in this
                select string.Format("[ {0}, {1} ]", wp.Coords.X, wp.Coords.Y);
            return string.Join("->", strings.ToArray());
        }
    }
}
