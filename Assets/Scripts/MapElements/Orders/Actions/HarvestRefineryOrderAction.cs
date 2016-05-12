using MechWars.MapElements.Orders.Actions.Args;

namespace MechWars.MapElements.Orders.Actions
{
    public class HarvestRefineryOrderAction : OrderAction<Unit>
    {
        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<RefineryOrderActionArgs>(args);
            return new HarvestOrder(orderExecutor,
                (Building)args[RefineryOrderActionArgs.RefineryArgName].Value);
        }
    }
}