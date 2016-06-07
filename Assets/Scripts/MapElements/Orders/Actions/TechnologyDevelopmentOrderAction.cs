using MechWars.MapElements.Orders_OLD.Products;

namespace MechWars.MapElements.Orders.Actions
{
    public class TechnologyDevelopmentOrderAction : ProductionOrderAction
    {
        public Technology technology;

        public override bool AllowsMultiExecutor { get { return false; } }

        protected override Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            if (technology == null)
                throw new System.Exception("\"Technology technology\" field must not be NULL.");
            AssertOrderExecutorIs<Building>(orderExecutor);
            return null;
            //return new TechnologyDevelopmentOrder((Building)orderExecutor,
            //    new TechnologyProduct(technology, cost, productionTime));
        }
    }
}