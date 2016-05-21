using MechWars.PlayerInput;

namespace MechWars.MapElements.Orders.Actions
{
    public interface ICanCreateOrderArgs
    {
        IBuildingPlacement BuildingPlacement { get; }
        Player Player { get; }
    }
}