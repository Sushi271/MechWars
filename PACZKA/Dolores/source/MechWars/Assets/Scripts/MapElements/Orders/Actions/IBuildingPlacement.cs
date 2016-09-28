using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public interface IBuildingPlacement
    {
        bool InsideMap { get; }
        bool CannotBuild { get; }
        Vector2 Position { get; }
    }
}
