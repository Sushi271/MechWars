using MechWars.Human;
using MechWars.PlayerInput;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions
{
    public class EscortOrderAction : OrderAction<Unit>
    {
        public override bool AllowsMultiTarget { get { return true; } }
        public override bool AllowsHover { get { return true; } }
        public override bool CanBeCarried { get { return true; } }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Escort(player, candidates);
        }

        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            var unitTargets = TryExtractTargetsArg<Unit>(args);
            return new EscortOrder(orderExecutor, unitTargets);
        }
    }
}