using MechWars.PlayerInput;

namespace MechWars.MapElements.Orders.Actions
{
    public interface ICanCreateOrderArgs
    {
        BuildingShadow BuildingShadow { get; }
        Player Player { get; }
    }
}