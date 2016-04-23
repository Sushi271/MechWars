using MechWars;
using MechWars.MapElements.Orders;

namespace MechWars.MapElements
{
    public class Building : MapElement
    {
        public bool isResourceDeposit;

        public QueueOrderExecutor OrderExecutor { get; private set; }
        public override bool Interactible { get { return true; } }

        public Building()
        {
            selectable = true;
            OrderExecutor = new QueueOrderExecutor();
        }

        public void GiveOrder(IOrder order)
        {
            if (order is Order<Building> || order is Order<MapElement>)
                OrderExecutor.Give(order);
            else throw new System.Exception(string.Format(
                "Order {0} not suitable for MapElement {1}.", order, this));
        }

        public void CancelOrder(IOrder order)
        {
            OrderExecutor.Cancel(order);
        }

        public void CancelAllOrders()
        {
            OrderExecutor.CancelAll();
        }
    }
}
