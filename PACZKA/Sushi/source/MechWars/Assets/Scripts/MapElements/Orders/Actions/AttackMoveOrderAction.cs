namespace MechWars.MapElements.Orders.Actions
{
    public class AttackMoveOrderAction : OrderAction
    {
        public override bool CanBeCarried { get { return true; } }

        protected override Order CreateOrder(MapElement orderExecutor, IOrderActionArgs orderActionArgs)
        {
            AssertOrderExecutorIs<Unit>(orderExecutor);
            return new AttackMoveOrder((Unit)orderExecutor, orderActionArgs.Destination);
        }
    }
}