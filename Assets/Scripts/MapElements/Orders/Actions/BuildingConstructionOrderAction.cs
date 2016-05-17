using MechWars.PlayerInput;
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

        public override bool CanCreateOrder(ICanCreateOrderArgs args)
        {
            if (args.BuildingShadow.InsideMap)
            {
                Debug.Log(string.Format("Cannot place building {0} outside map.", building));
                return false;
            }

            if (args.BuildingShadow.PositionOccupied)
            {
                Debug.Log(string.Format("Cannot place building {0} in location {1} - it's occupied.",
                    building, args.BuildingShadow.Position));
                return false;
            }

            if (StartCost > args.Player.Army.resources)
            {
                Debug.Log(string.Format("Not enough resources to start construction of building {0}.", building));
                return false;
            }

            return true;
        }

        public override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            if (building == null)
                throw new System.Exception("\"Building building\" field must not be NULL.");
            AssertOrderExecutorIs<Building>(orderExecutor);
            
            return new BuildingConstructionOrder((Building)orderExecutor, building);
        }

        public override OrderActionArgs CreateArgs(InputController inputController)
        {
            return new OrderActionArgs(
                inputController.Mouse.MapRaycast.Coords.Value,
                inputController.HoverController.HoveredMapElements);
        }
    }
}