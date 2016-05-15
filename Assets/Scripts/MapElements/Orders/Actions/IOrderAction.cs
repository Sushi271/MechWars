using MechWars.PlayerInput;

namespace MechWars.MapElements.Orders.Actions
{
    public interface IOrderAction : IMouseBehaviourDeterminant
    {
        bool TEMP_ForUnit { get; }
        // ---------------------------

        bool IsAttack { get; }
        bool CanBeCarried { get; }
        IOrder CreateOrder(MapElement orderExecutor, OrderActionArgs args);

    }
}
