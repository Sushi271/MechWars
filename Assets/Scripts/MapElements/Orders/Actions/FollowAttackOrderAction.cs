using MechWars.MapElements.Orders.Actions.Args;

namespace MechWars.MapElements.Orders.Actions
{
    public class FollowAttackOrderAction : OrderAction<Unit>
    {
        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            AssertOrderActionArgsTypeValid<TargetOrderActionArgs>(args);
            return new FollowAttackOrder(orderExecutor,
                (MapElement)args[TargetOrderActionArgs.TargetArgName].Value);
        }
    }
}