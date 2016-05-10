using MechWars.MapElements.Orders;
using MechWars.Utils;

namespace MechWars.MapElements.OrderActions
{
    public class UnitProductionOrderAction : OrderAction
    {
        public const string OrderedBuildingArgName = "orderedBuilding";
        public const string ProducedUnitArgName = "producedUnit";

        public override IOrder CreateOrder(OrderActionArgs args)
        {
            return new UnitProductionOrder(
                (Building)args[OrderedBuildingArgName],
                (Unit)args[ProducedUnitArgName]);
        }
    }
}