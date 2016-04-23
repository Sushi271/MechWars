namespace MechWars.MapElements.Orders
{
    public class SingleOrderExecutor
    {
        public IOrder CurrentOrder { get; private set; }

        IOrder nextOrder;

        public void Give(IOrder order)
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
                CurrentOrder.Update();
                if (CurrentOrder.Stopped)
                {
                    CurrentOrder = nextOrder;
                    nextOrder = null;
                }                
            }
        }
    }
}
