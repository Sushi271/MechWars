using MechWars.MapElements.Orders;
using MechWars.Utils;

namespace MechWars.MapElements.OrderActions
{
    public class BuildingConstructionOrderAction : OrderAction
    {
        public const string OrderedBuildingArgName = "orderedBuilding";
        public const string ConstructedBuildingArgName = "constructedBuilding";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            return new BuildingConstructionOrder(
                (Building)args[OrderedBuildingArgName],
                (Building)args[ConstructedBuildingArgName]);
        }
    }
}