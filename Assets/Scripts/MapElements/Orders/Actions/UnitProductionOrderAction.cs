namespace MechWars.MapElements.Orders.Actions
{
    public class UnitProductionOrderAction : ProductionOrderAction
    {
        public Unit unit;

        public override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            if (unit == null)
                throw new System.Exception("\"Unit unit\" field must not be NULL.");
            AssertOrderExecutorIs<Building>(orderExecutor);
            return new UnitProductionOrder((Building)orderExecutor, unit);
        }
    }
}