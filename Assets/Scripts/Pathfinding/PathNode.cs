namespace MechWars.Pathfinding
{
    public class WayPoint
    {
        public CoordPair Coords { get; private set; }
        public WayPoint Next { get; private set; }

        public WayPoint(CoordPair coords, WayPoint next)
        {
            Coords = coords;
            Next = next;
        }
    }
}