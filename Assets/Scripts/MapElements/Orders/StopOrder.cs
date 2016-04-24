using System;

namespace MechWars.MapElements.Orders
{
    public class StopOrder : Order<MapElement>
    {
        public StopOrder(MapElement orderedUnit)
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

        protected override void TerminateCore()
        {
        }
    }
}