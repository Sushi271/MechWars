using MechWars.MapElements.Orders.Actions.Args;

namespace MechWars.MapElements.Orders.Actions
{
    public class BuildingStandAttackOrderAction : OrderAction<Building>
    {
        public override IOrder CreateOrder(Building orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<TargetOrderActionArgs>(args);
            return new StandAttackOrder(orderExecutor,
                (MapElement)args[TargetOrderActionArgs.TargetArgName].Value);
        }
    }
}