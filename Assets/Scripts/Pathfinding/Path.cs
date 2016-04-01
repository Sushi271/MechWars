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

        public WayPoint First { get { return Count == 0 ? null : wayPoints[Count - 1]; } }

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
            wayPoints.RemoveAt(Count - 1);
            // TODO: Error check, update Length!
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
