using MechWars.MapElements.Orders;
using MechWars.Utils;

namespace MechWars.MapElements.OrderActions
{
    public class AttackMoveOrderAction : OrderAction
    {
        public const string OrderedUnitArgName = "orderedUnit";
        public const string DestinationArgName = "destination";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            return new AttackMoveOrder(
                (Unit)args[OrderedUnitArgName],
                (IVector2)args[DestinationArgName]);
        }
    }
}