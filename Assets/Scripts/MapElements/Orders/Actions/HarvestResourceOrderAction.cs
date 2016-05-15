using MechWars.Utils;
using System.Linq;

namespace MechWars.MapElements.Orders.Actions
{
    public class HarvestResourceOrderAction : OrderAction<Unit>
    {
        public override bool CanBeCarried { get { return true; } }

        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            var resourceTargets = TryExtractTargetsArg<Resource>(args);
            if (resourceTargets.Empty())
                throw new System.Exception(
                    "HarvestResourceOrderAction requires single Resource target, but none provided.");
            if (resourceTargets.HasAtLeast(2))
                throw new System.Exception(
                    "HarvestResourceOrderAction requires single Resource target, but more than 1 provided.");
            return new HarvestOrder(orderExecutor, resourceTargets.First());
        }
    }
}