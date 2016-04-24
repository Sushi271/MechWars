namespace MechWars.MapElements.Orders
{
    public interface IOrder
    {
        string Name { get; }
        MapElement MapElement { get; }
        bool Stopping { get; }
        bool Stopped { get; }

        void Update();
        void Stop();
        void Terminate();
    }
}