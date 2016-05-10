using MechWars.MapElements.Orders;
using MechWars.Utils;

namespace MechWars.MapElements.OrderActions
{
    public class MoveOrderAction : OrderAction
    {
        public const string OrderedUnitArgName = "orderedUnit";
        public const string DestinationArgName = "destination";
        public const string DontStopArgName = "dontStop";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            object outDontStop;
            bool success = args.TryGetArg(DontStopArgName, out outDontStop);
            bool dontStop = outDontStop != null && outDontStop is bool && (bool)outDontStop == true;
            return new MoveOrder(
                (Unit)args[OrderedUnitArgName],
                (IVector2)args[DestinationArgName],
                dontStop);
        }
    }
}