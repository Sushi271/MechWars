using System.Collections.Generic;
using System.Linq;

namespace MechWars.MapElements.Orders
{
    public class QueueOrderExecutor
    {
        List<IOrder> orderQueue;

        public int Count { get { return orderQueue.Count; } }
        public IOrder CurrentOrder { get { return Count == 0 ? null : orderQueue.First(); } }
        public IOrder this[int i] { get { return orderQueue[i]; } }

        public QueueOrderExecutor()
        {
            orderQueue = new List<IOrder>();
        }
        
        public void Give(IOrder order)
        {
            orderQueue.Add(order);
        }

        public void Cancel(int idx)
        {
            if (idx < 0 || Count <= idx)
                throw new System.IndexOutOfRangeException(
                    "Parameter idx must be between 0 (inclusive) and Count (exclusive).");
            if (idx > 0)
            {
                orderQueue[idx].Terminate();
                orderQueue.RemoveAt(idx);
            }
            else if (!CurrentOrder.Stopped && !CurrentOrder.Stopping)
                CurrentOrder.Stop();
        }

        public void Cancel(IOrder order)
        {
            int idx = orderQueue.IndexOf(order);
            if (idx == -1)
                throw new System.ArgumentException(string.Format(
                    "QueueOrderExecutor does not contain {0} order to cancel.", order), "order");
            Cancel(idx);
        }

        public void CancelAll()
        {
            if (Count == 0) return;
            while (Count > 1)
            {
                orderQueue[Count - 1].Terminate();
                orderQueue.RemoveAt(Count - 1);
            }
            Cancel(0);
        }

        public void Update()
        {
            if (CurrentOrder != null)
            {
                CurrentOrder.Update();
                if (CurrentOrder.Stopped)
                    orderQueue.RemoveAt(0);
            }
        }

        public void Terminate()
        {
            foreach (var o in orderQueue)
                o.Terminate();
            orderQueue.Clear();
        }
    }
}
