using System.Collections.Generic;
using System.Linq;

namespace MechWars.MapElements
{
    public class MapElementGroup
    {
        IEnumerable<MapElement> mapElements;
        
        public UnitGroup Units {  get { return new UnitGroup(mapElements.Where(me => me is Unit).Cast<Unit>()); } }
        public BuildingGroup Buildings { get { return new BuildingGroup(mapElements.Where(me => me is Building).Cast<Building>()); } }

        public MapElementGroup(IEnumerable<MapElement> mapElements)
        {
            this.mapElements = mapElements;
        }

        public MapElementGroup Where(System.Func<MapElement, bool> predicate)
        {
            return new MapElementGroup(mapElements.Where(predicate));
        }
    }
}
