using MechWars.MapElements.Orders;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.MapElements
{
    public class UnitGroup
    {
        IEnumerable<Unit> units;

        public UnitGroup(IEnumerable<Unit> units)
        {
            this.units = units;
        }

        public UnitGroup Where(System.Func<Unit, bool> predicate)
        {
            return new UnitGroup(units.Where(predicate));
        }

        public void GiveOrder(System.Func<Unit, Order<Unit>> orderConstructor)
        {
            foreach (var u in units)
                u.GiveOrder(orderConstructor(u));
        }
    }
}
