using MechWars.MapElements.Orders;
using MechWars.Utils;

namespace MechWars.MapElements.OrderActions
{
    public class TechnologyDevelopmentOrderAction : OrderAction
    {
        public const string OrderedBuildingArgName = "orderedBuilding";
        public const string DevelopedTechnologyArgName = "developedTechnology";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            return new TechnologyDevelopmentOrder(
                (Building)args[OrderedBuildingArgName],
                (Technology)args[DevelopedTechnologyArgName]);
        }
    }
}