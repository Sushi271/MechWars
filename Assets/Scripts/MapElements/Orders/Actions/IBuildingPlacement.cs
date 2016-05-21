using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public interface IBuildingPlacement
    {
        bool InsideMap { get; }
        bool PositionOccupied { get; }
        Vector2 Position { get; }
    }
}
