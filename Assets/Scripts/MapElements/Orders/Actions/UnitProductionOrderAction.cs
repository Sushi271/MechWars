using MechWars.MapElements.Orders_OLD.Products;

namespace MechWars.MapElements.Orders.Actions
{
    public class UnitProductionOrderAction : ProductionOrderAction
    {
        public Unit unit;

        protected override Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            if (unit == null)
                throw new System.Exception("\"Unit unit\" field must not be NULL.");
            AssertOrderExecutorIs<Building>(orderExecutor);
            return null;
            //return new UnitProductionOrder((Building)orderExecutor,
            //    new UnitProduct(unit, cost, productionTime));
        }
    }
}