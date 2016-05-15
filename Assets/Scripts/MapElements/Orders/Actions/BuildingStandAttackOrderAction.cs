using MechWars.Human;
using MechWars.PlayerInput;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions
{
    public class BuildingStandAttackOrderAction : OrderAction<Building>
    {
        public override bool AllowsMultiTarget { get { return true; } }
        public override bool AllowsHover { get { return true; } }
        public override bool IsAttack { get { return true; } }
        public override bool CanBeCarried { get { return true; } }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Attack(player, candidates);
        }

        public override IOrder CreateOrder(Building orderExecutor, OrderActionArgs args)
        {
            return new StandAttackOrder(orderExecutor, args.Targets);
        }
    }
}