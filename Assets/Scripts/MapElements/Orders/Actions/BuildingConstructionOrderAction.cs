using MechWars.MapElements.Orders.Actions.Args;

namespace MechWars.MapElements.Orders.Actions
{
    public class BuildingConstructionOrderAction : OrderAction<Building>
    {
        public override IOrder CreateOrder(Building orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<BuildingOrderActionArgs>(args);
            return new BuildingConstructionOrder(orderExecutor,
                (Building)args[BuildingOrderActionArgs.BuildingArgName].Value);
        }
    }
}