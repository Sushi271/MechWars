using MechWars.MapElements.Orders.Actions.Args;

namespace MechWars.MapElements.Orders.Actions
{
    public class TechnologyDevelopmentOrderAction : OrderAction<Building>
    {
        public override IOrder CreateOrder(Building orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<TechnologyOrderActionArgs>(args);
            return new TechnologyDevelopmentOrder(orderExecutor,
                (Technology)args[TechnologyOrderActionArgs.TechnologyArgName].Value);
        }
    }
}