using MechWars.Utils;
using System.Linq;

namespace MechWars.MapElements.Orders.Actions
{
    public class HarvestRefineryOrderAction : OrderAction<Unit>
    {
        public override bool CanBeCarried { get { return true; } }

        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            var buildingTargets = TryExtractTargetsArg<Building>(args);
            if (buildingTargets.Empty())
                throw new System.Exception(
                    "HarvestRefineryOrderAction requires single Builiding target, but none provided.");
            if (buildingTargets.HasAtLeast(2))
                throw new System.Exception(
                    "HarvestRefineryOrderAction requires single Builiding target, but more than 1 provided.");
            return new HarvestOrder(orderExecutor, buildingTargets.First());
        }
    }
}