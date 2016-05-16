namespace MechWars.MapElements.Orders.Actions
{
    public class StopOrderAction : OrderAction
    {
        public override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            return new StopOrder(orderExecutor);
        }
    }
}