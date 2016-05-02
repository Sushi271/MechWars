using MechWars.MapElements.Orders;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.MapElements
{
    public class BuildingGroup
    {
        IEnumerable<Building> buildings;

        public BuildingGroup(IEnumerable<Building> units)
        {
            this.buildings = units;
        }

        public BuildingGroup Where(System.Func<Building, bool> predicate)
        {
            return new BuildingGroup(buildings.Where(predicate));
        }

        public void GiveOrder(System.Func<Building, Order<Building>> orderConstructor)
        {
            foreach (var b in buildings)
                b.GiveOrder(orderConstructor(b));
        }

        public void GiveOrder(System.Func<Building, Order<MapElement>> orderConstructor)
        {
            foreach (var b in buildings)
                b.GiveOrder(orderConstructor(b));
        }
    }
}
