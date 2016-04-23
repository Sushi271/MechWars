namespace MechWars.MapElements.Orders
{
    public class StopOrder : Order
    {
        public StopOrder(Unit orderedUnit)
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