using MechWars.MapElements.Orders;

namespace MechWars.MapElements.OrderActions
{
    public class FollowAttackOrderAction : OrderAction
    {
        public const string OrderedUnitArgName = "orderedUnit";
        public const string TargetArgName = "target";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            return new FollowAttackOrder(
                (Unit)args[OrderedUnitArgName],
                (MapElement)args[TargetArgName]);
        }
    }
}