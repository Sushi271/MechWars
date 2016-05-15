namespace MechWars.MapElements.Orders.Actions
{
    public class AttackMoveOrderAction : OrderAction<Unit>
    {
        public override bool CanBeCarried { get { return true; } }

        public override IOrder CreateOrder(Unit orderExecutor, OrderActionArgs args)
        {
            return new AttackMoveOrder(orderExecutor, args.Destination);
        }
    }
}