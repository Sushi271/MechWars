using MechWars.MapElements.Orders;
using MechWars.Utils;

namespace MechWars.MapElements.OrderActions
{
    public class StandAttackOrderAction : OrderAction
    {
        public const string OrderedBuildingArgName = "orderedBuilding";
        public const string OrderedUnitArgName = "orderedUnit";
        public const string TargetArgName = "target";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            object orderedBuilding;
            bool success = args.TryGetArg(OrderedBuildingArgName, out orderedBuilding);
            if (success)
                return new StandAttackOrder(
                    (Building)orderedBuilding,
                    (MapElement)args[TargetArgName]);
            else
                return new StandAttackOrder(
                    (Unit)args[OrderedUnitArgName],
                    (MapElement)args[TargetArgName]);
        }
    }
}