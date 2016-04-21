using MechWars.MapElements;

namespace MechWars.Pathfinding
{
    public interface IPathfinder
    {
        Path FindPath(CoordPair start, CoordPair target, Unit orderedUnit);
    }
}
