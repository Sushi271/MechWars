using MechWars.Human;
using MechWars.PlayerInput;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions
{
    public class FollowAttackOrderAction : OrderAction
    {
        public override bool AllowsMultiTarget { get { return true; } }
        public override bool AllowsHover { get { return true; } }
        public override bool IsAttack { get { return true; } }
        public override bool CanBeCarried { get { return true; } }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Attack(player, candidates);
        }

        public override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            AssertOrderExecutorIs<Unit>(orderExecutor);
            return new FollowAttackOrder((Unit)orderExecutor, args.Targets);
        }
    }
}