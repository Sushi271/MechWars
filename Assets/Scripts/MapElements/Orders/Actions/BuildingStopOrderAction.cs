namespace MechWars.MapElements.Orders.Actions
{
    public class BuildingStopOrderAction : OrderAction<Building>
    {
        public override IOrder CreateOrder(Building orderExecutor, OrderActionArgs args)
        {
            return new StopOrder(orderExecutor);
        }
    }
}