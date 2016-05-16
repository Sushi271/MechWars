using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public class BuildingConstructionOrderAction : ProductionOrderAction
    {
        public Building building;

        public override bool CanBeCarried { get { return true; } }

        public int StartCost { get { return Mathf.CeilToInt(cost * Globals.Instance.startingBuildingProgress); } }

        public override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            if (building == null)
                throw new System.Exception("\"Building building\" field must not be NULL.");
            AssertOrderExecutorIs<Building>(orderExecutor);
            return new BuildingConstructionOrder((Building)orderExecutor, building);
        }
    }
}