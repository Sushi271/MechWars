using MechWars.MapElements.Orders.Actions.Args;

namespace MechWars.MapElements.Orders.Actions
{
    public class EscortOrderAction : OrderAction<Unit>
    {
        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<UnitTargetOrderActionArgs>(args);
            return new EscortOrder(orderExecutor,
                (Unit)args[UnitTargetOrderActionArgs.TargetArgName].Value);
        }
    }
}