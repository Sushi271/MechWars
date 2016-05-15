using MechWars.MapElements.Orders.Actions.Args;
using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public class BuildingConstructionOrderAction : ProductionOrderAction
    {
        public Building building;

        public int StartCost { get { return Mathf.CeilToInt(cost * Globals.Instance.startingBuildingProgress); } }

        public override IOrder CreateOrder(Building orderExecutor, OrderActionArgs args)
        {
            if (building == null)
                throw new System.Exception("\"Building building\" field must not be NULL.");
            return new BuildingConstructionOrder(orderExecutor, building);
        }
    }
}