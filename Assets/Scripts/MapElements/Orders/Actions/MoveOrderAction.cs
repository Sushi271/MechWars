using MechWars.MapElements.Orders.Actions.Args;
using MechWars.Utils;

namespace MechWars.MapElements.Orders.Actions
{
    public class MoveOrderAction : OrderAction<Unit>
    {
        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<DestinationOrderActionArgs>(args);
            return new MoveOrder(orderExecutor,
                (IVector2)args[DestinationOrderActionArgs.DestinationArgName].Value);
        }
    }
}