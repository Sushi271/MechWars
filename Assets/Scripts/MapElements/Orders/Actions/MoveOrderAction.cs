namespace MechWars.MapElements.Orders.Actions
{
    public class MoveOrderAction : OrderAction
    {
        public override bool CanBeCarried { get { return true; } }

        protected override Order CreateOrder(MapElement orderExecutor, OrderActionArgs args)
        {
            AssertOrderExecutorIs<Unit>(orderExecutor);
            return new MoveOrder((Unit)orderExecutor, args.Destination);
        }
    }
}