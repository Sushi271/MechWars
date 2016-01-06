using System.Collections.Generic;
using MechWars.MapElements;

namespace MechWars.Orders
{
    public class Stop : Order
    {
        public Stop(List<Unit> orderedUnits)
            : base("Stop", orderedUnits)
        {
        }

        protected override bool RegularUpdate(Unit unit)
        {
            return true;
        }
        
        protected override bool StoppingUpdate(Unit unit)
        {
            return true;
        }
    }
}
