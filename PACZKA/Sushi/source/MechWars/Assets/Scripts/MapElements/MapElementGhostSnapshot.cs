using MechWars.MapElements.Statistics;

namespace MechWars.MapElements
{
    public class MapElementGhostSnapshot
    {
        public Stats Stats { get; private set; }
        public Army Army { get; private set; }

        public MapElementGhostSnapshot(MapElement original, MapElement newMapElement)
        {
            Stats = original.Stats.Clone(newMapElement);
            Army = original.Army;
        }
    }
}