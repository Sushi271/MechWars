using MechWars.PlayerInput;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions
{
    public class StandAttackOrderAction : OrderAction
    {
        public override bool AllowsMultiTarget { get { return true; } }
        public override bool AllowsHover { get { return true; } }
        public override bool IsAttack { get { return true; } }
        public override bool CanBeCarried { get { return true; } }

        public override void FilterHoverCandidates(HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Attack(candidates);
        }

        protected override bool CanCreateOrder(IOrderActionArgs orderActionArgs)
        {
            return !orderActionArgs.Targets.Empty();
        }

        protected override Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            AssertOrderExecutorIs<Unit, Building>(orderExecutor);
            return new StandAttackOrder(orderExecutor, orderActionArgs.Targets);
        }
    }
}