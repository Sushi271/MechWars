using MechWars.Utils;
using System.Linq;

namespace MechWars.MapElements.Orders.Actions
{
    public class HarvestResourceOrderAction : OrderAction
    {
        public override bool CanBeCarried { get { return true; } }

        public override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            AssertOrderExecutorIs<Unit>(orderExecutor);
            var resourceTargets = TryExtractTargetsArg<Resource>(args);
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