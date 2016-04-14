using System.Collections.Generic;
using MechWars.MapElements;

namespace MechWars.Orders
{
    public class Stop : Order
    {
        public Stop(Unit orderedUnit)
            : base("Stop", orderedUnit)
        {
        }

        protected override bool RegularUpdate()
        {
            return true;
        }
        
        protected override bool StoppingUpdate()
        {
            return true;
        }
    }
}
