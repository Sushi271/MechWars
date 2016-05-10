using MechWars.MapElements.Orders;

namespace MechWars.MapElements.OrderActions
{
    public class StopOrderAction : OrderAction
    {
        public const string OrderedMapElementArgName = "orderedMapElement";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            return new StopOrder((MapElement)args[OrderedMapElementArgName]);
        }
    }
}