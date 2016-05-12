using MechWars.MapElements.Orders.Actions.Args;

namespace MechWars.MapElements.Orders.Actions
{
    public class UnitProductionOrderAction : OrderAction<Building>
    {
        public override IOrder CreateOrder(Building orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<UnitOrderActionArgs>(args);
            return new UnitProductionOrder(orderExecutor,
                (Unit)args[UnitOrderActionArgs.UnitArgName].Value);
        }
    }
}