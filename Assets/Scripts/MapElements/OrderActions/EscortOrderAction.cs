using MechWars.MapElements.Orders;

namespace MechWars.MapElements.OrderActions
{
    public class EscortOrderAction : OrderAction
    {
        public const string OrderedUnitArgName = "orderedUnit";
        public const string TargetArgName = "target";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            return new EscortOrder(
                (Unit)args[OrderedUnitArgName],
                (Unit)args[TargetArgName]);
        }
    }
}