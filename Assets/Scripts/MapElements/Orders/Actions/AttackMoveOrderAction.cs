using MechWars.MapElements.Orders.Actions.Args;
using MechWars.Utils;

namespace MechWars.MapElements.Orders.Actions
{
    public class AttackMoveOrderAction : OrderAction<Unit>
    {
        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<DestinationOrderActionArgs>(args);
            return new AttackMoveOrder(orderExecutor,
                (IVector2)args[DestinationOrderActionArgs.DestinationArgName].Value);
        }
    }
}