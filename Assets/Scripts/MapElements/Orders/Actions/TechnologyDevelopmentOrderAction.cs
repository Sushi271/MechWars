namespace MechWars.MapElements.Orders.Actions
{
    public class TechnologyDevelopmentOrderAction : ProductionOrderAction
    {
        public Technology technology;

        public override IOrder CreateOrder(Building orderExecutor, OrderActionArgs args)
        {
            if (technology == null)
                throw new System.Exception("\"Technology technology\" field must not be NULL.");
            return new TechnologyDevelopmentOrder(orderExecutor, technology);
        }
    }
}