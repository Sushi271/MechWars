using MechWars.MapElements.Orders.Actions;
using UnityEngine;

namespace MechWars.AI
{
    public class AIBuildingPlacement : IBuildingPlacement
    {
        public bool CannotBuild { get; private set; }
        public bool InsideMap { get { return true; } }
        public Vector2 Position { get; private set; }

        public AIBuildingPlacement(bool cannotBuild)
        {
            CannotBuild = cannotBuild;
        }

        public AIBuildingPlacement(Vector2 position)
        {
            Position = position;
        }
    }
}