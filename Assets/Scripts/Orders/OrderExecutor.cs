using MechWars.MapElements;

namespace MechWars.Orders
{
    public class OrderExecutor
    {
        public Unit Unit { get; private set; }

        public Order CurrentOrder { get; private set; }

        Order nextOrder;

        public OrderExecutor(Unit unit)
        {
            Unit = unit;
        }

        public void Give(Order order)
        {
            if (CurrentOrder != null)
            {
                if (!CurrentOrder.Stopped && !CurrentOrder.Stopping)
                    CurrentOrder.Stop();
                nextOrder = order;
            }
            else CurrentOrder = order;
        }

        public void Update()
        {
            if (CurrentOrder != null)
            {
                CurrentOrder.Update(Unit);
                if (CurrentOrder.Stopped)
                {
                    CurrentOrder = nextOrder;
                    nextOrder = null;
                }                
            }
        }
    }
}
