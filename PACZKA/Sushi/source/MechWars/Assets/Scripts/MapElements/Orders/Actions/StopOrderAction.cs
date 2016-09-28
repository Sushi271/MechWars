namespace MechWars.MapElements.Orders.Actions
{
    public class StopOrderAction : OrderAction
    {
        protected override Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            return new StopOrder(orderExecutor);
        }
    }
}