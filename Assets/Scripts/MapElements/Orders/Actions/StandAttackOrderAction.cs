using MechWars.Human;
using MechWars.PlayerInput;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions
{
    public class StandAttackOrderAction : OrderAction
    {
        public override bool AllowsMultiTarget { get { return true; } }
        public override bool AllowsHover { get { return true; } }
        public override bool IsAttack { get { return true; } }
        public override bool CanBeCarried { get { return true; } }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Attack(player, candidates);
        }

        protected override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            AssertOrderExecutorIs<Unit, Building>(orderExecutor);
            return new StandAttackOrder(orderExecutor, args.Targets);
        }
    }
}