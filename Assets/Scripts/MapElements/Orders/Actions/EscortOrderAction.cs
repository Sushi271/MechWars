using MechWars.PlayerInput;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions
{
    public class EscortOrderAction : OrderAction
    {
        public override bool AllowsMultiTarget { get { return true; } }
        public override bool AllowsHover { get { return true; } }
        public override bool IsEscort { get { return true; } }
        public override bool CanBeCarried { get { return true; } }

        public override void FilterHoverCandidates(HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Escort(candidates);
        }

        protected override bool CanCreateOrder(IOrderActionArgs orderActionArgs)
        {
            return !orderActionArgs.Targets.Empty();
        }

        protected override Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            AssertOrderExecutorIs<Unit>(orderExecutor);
            var unitTargets = TryExtractTargetsArg<Unit>(orderActionArgs);
            return new EscortOrder((Unit)orderExecutor, unitTargets);
        }
    }
}