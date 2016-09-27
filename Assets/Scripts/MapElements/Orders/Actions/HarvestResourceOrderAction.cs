using MechWars.Utils;
using System.Linq;
using System.Collections.Generic;
using MechWars.PlayerInput;

namespace MechWars.MapElements.Orders.Actions
{
    public class HarvestResourceOrderAction : OrderAction
    {
        public override bool AllowsHover { get { return true; } }
        public override bool CanBeCarried { get { return true; } }

        public override void FilterHoverCandidates(HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.HarvestResource(candidates);
        }

        protected override bool CanCreateOrder(IOrderActionArgs orderActionArgs)
        {
            return orderActionArgs.Targets.HasExactly(1);
        }

        protected override Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            AssertOrderExecutorIs<Unit>(orderExecutor);
            var resourceTargets = TryExtractTargetsArg<Resource>(orderActionArgs);
            if (resourceTargets.Empty())
                throw new System.Exception(
                    "HarvestResourceOrderAction requires single Resource target, but none provided.");
            if (resourceTargets.HasAtLeast(2))
                throw new System.Exception(
                    "HarvestResourceOrderAction requires single Resource target, but more than 1 provided.");
            return new HarvestOrder((Unit)orderExecutor, resourceTargets.First());
        }
    }
}