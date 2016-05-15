using MechWars.MapElements.Orders.Actions.Args;

namespace MechWars.MapElements.Orders.Actions
{
    public class UnitProductionOrderAction : ProductionOrderAction
    {
        public Unit unit;

        public override IOrder CreateOrder(Building orderExecutor, OrderActionArgs args)
        {
            if (unit == null)
                throw new System.Exception("\"Unit unit\" field must not be NULL.");
            return new UnitProductionOrder(orderExecutor, unit);
        }
    }
}