using MechWars.MapElements.Orders.Actions.Args;

namespace MechWars.MapElements.Orders.Actions
{
    public class HarvestResourceOrderAction : OrderAction<Unit>
    {
        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<ResourceOrderActionArgs>(args);
            return new HarvestOrder(orderExecutor,
                (Resource)args[ResourceOrderActionArgs.ResourceArgName].Value);
        }
    }
}