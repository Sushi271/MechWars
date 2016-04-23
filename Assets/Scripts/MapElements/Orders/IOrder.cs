namespace MechWars.MapElements.Orders
{
    public interface IOrder
    {
        string Name { get; }
        Unit Unit { get; }
        bool Stopping { get; }
        bool Stopped { get; }
    }
}