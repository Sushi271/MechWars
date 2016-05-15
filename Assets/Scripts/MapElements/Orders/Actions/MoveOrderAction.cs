namespace MechWars.MapElements.Orders.Actions
{
    public class MoveOrderAction : OrderAction<Unit>
    {
        public override bool CanBeCarried { get { return true; } }

        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            return new MoveOrder(orderExecutor, args.Destination);
        }
    }
}