using MechWars.MapElements.Orders;

namespace MechWars.MapElements.OrderActions
{
    public class HarvestOrderAction : OrderAction
    {
        public const string OrderedUnitArgName = "orderedUnit";
        public const string ResourceArgName = "resource";
        public const string RefineryArgName = "refinery";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            object resource;
            bool success = args.TryGetArg(ResourceArgName, out resource);

            if (success)
                return new HarvestOrder(
                    (Unit)args[OrderedUnitArgName],
                    (Resource)resource);
            else
                return new HarvestOrder(
                    (Unit)args[OrderedUnitArgName],
                    (Building)args[RefineryArgName]);
        }
    }
}