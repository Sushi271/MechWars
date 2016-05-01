namespace MechWars.MapElements.Orders
{
    public class SingleOrderExecutor
    {
        System.Func<IOrder> defaultOrderCreator;
        
        public IOrder CurrentOrder { get; private set; }

        IOrder nextOrder;

        public SingleOrderExecutor(System.Func<IOrder> defaultOrderCreator = null)
        {
            if (defaultOrderCreator == null)
                defaultOrderCreator = () => null;
            this.defaultOrderCreator = defaultOrderCreator;
            nextOrder = defaultOrderCreator();
        }

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
                    nextOrder = defaultOrderCreator();
                }
            }
            else CurrentOrder = defaultOrderCreator();
        }

        public void Terminate()
        {
            nextOrder = null;
            if (CurrentOrder != null)
            {
                CurrentOrder.Terminate();
                CurrentOrder = null;
            }
        }
    }
}
