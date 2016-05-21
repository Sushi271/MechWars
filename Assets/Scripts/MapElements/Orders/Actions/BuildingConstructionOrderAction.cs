using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public class BuildingConstructionOrderAction : ProductionOrderAction
    {
        public Building building;

        public override bool AllowsMultiExecutor { get { return false; } }
        public override bool CanBeCarried { get { return true; } }
        public override bool IsSequential { get { return true; } }

        public int StartCost { get { return Mathf.CeilToInt(cost * Globals.Instance.startingBuildingProgress); } }

        protected override bool CanCreateOrder(ICanCreateOrderArgs canCreateOrderArgs)
        {
            if (canCreateOrderArgs.BuildingPlacement.InsideMap)
            {
                Debug.Log(string.Format("Cannot place building {0} outside map.", building));
                return false;
            }

            if (canCreateOrderArgs.BuildingPlacement.PositionOccupied)
            {
                Debug.Log(string.Format("Cannot place building {0} in location {1} - it's occupied.",
                    building, canCreateOrderArgs.BuildingPlacement.Position));
                return false;
            }

            if (StartCost > canCreateOrderArgs.Player.Army.resources)
            {
                Debug.Log(string.Format("Not enough resources to start construction of building {0}.", building));
                return false;
            }

            return true;
        }

        protected override Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            if (building == null)
                throw new System.Exception("\"Building building\" field must not be NULL.");
            AssertOrderExecutorIs<Building>(orderExecutor);
            
            return new BuildingConstructionOrder((Building)orderExecutor, building);
        }
    }
}