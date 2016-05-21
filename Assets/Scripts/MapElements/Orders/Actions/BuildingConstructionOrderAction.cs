using MechWars.MapElements.Orders.Products;
using MechWars.PlayerInput;
using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public class BuildingConstructionOrderAction : ProductionOrderAction, IBuildingConstructArgs
    {
        public Building building;
        public bool sequential;

        public Building Building { get { return building; } }
        public int Cost { get { return cost; } }
        public float ProductionTime { get { return productionTime; } }

        public override bool AllowsMultiExecutor { get { return false; } }
        public override bool CanBeCarried { get { return true; } }
        public override bool IsSequential { get { return sequential; } }

        public int StartCost { get { return Mathf.CeilToInt(cost * Globals.MapSettings.startingBuildingProgress); } }

        protected override bool CanCreateOrder(IOrderActionArgs orderActionArgs)
        {
            if (!orderActionArgs.BuildingPlacement.InsideMap)
            {
                Debug.Log(string.Format("Cannot place building {0} outside map.", building));
                return false;
            }

            if (orderActionArgs.BuildingPlacement.PositionOccupied)
            {
                Debug.Log(string.Format("Cannot place building {0} in location {1} - it's occupied.",
                    building, orderActionArgs.BuildingPlacement.Position));
                return false;
            }

            if (StartCost > orderActionArgs.Player.army.resources)
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

            Building constructor = (Building)orderExecutor;
            BuildingProduct buildingProduct = constructor.Construct(this, orderActionArgs.BuildingPlacement.Position);


            return new BuildingConstructionOrder((Building)orderExecutor, buildingProduct);
        }
    }
}