namespace MechWars.Pathfinding
{
    public interface IPathfinder
    {
        Path FindPath(CoordPair start, CoordPair target, MechWars.MapElements.Unit unit);
    }
}
