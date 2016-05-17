namespace MechWars.MapElements.Orders.Actions
{
    public class TechnologyDevelopmentOrderAction : ProductionOrderAction
    {
        public Technology technology;

        public override bool AllowsMultiExecutor { get { return false; } }

        public override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            if (technology == null)
                throw new System.Exception("\"Technology technology\" field must not be NULL.");
            AssertOrderExecutorIs<Building>(orderExecutor);
            return new TechnologyDevelopmentOrder((Building)orderExecutor, technology);
        }
    }
}