using MechWars.Utils;
using System.Linq;

namespace MechWars.MapElements.Orders.Actions
{
    public class HarvestRefineryOrderAction : OrderAction
    {
        public override bool CanBeCarried { get { return true; } }

        protected override bool CanCreateOrder(IOrderActionArgs orderActionArgs)
        {
            return orderActionArgs.Targets.HasExactly(1);
        }

        protected override Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            AssertOrderExecutorIs<Unit>(orderExecutor);
            var buildingTargets = TryExtractTargetsArg<Building>(orderActionArgs);
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