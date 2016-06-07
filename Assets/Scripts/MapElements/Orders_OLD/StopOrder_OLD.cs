using System;

namespace MechWars.MapElements.Orders_OLD
{
    public class StopOrder_OLD : Order_OLD
    {
        public StopOrder_OLD(MapElement orderedMapElement)
            : base("Stop", orderedMapElement)
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