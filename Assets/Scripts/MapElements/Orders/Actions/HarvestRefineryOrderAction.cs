using MechWars.Utils;
using System.Linq;

namespace MechWars.MapElements.Orders.Actions
{
    public class HarvestRefineryOrderAction : OrderAction
    {
        public override bool CanBeCarried { get { return true; } }

        public override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            AssertOrderExecutorIs<Unit>(orderExecutor);
            var buildingTargets = TryExtractTargetsArg<Building>(args);
            if (buildingTargets.Empty())
                throw new System.Exception(
                    "HarvestRefineryOrderAction requires single Builiding target, but none provided.");
            if (buildingTargets.HasAtLeast(2))
                throw new System.Exception(
                    "HarvestRefineryOrderAction requires single Builiding target, but more than 1 provided.");
            return new HarvestOrder((Unit)orderExecutor, buildingTargets.First());
        }
    }
}