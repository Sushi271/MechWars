using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: add / replace (join functionalities of Queue and Single)

namespace MechWars.MapElements.Orders
{
    public class OrderExecutor
    {
        List<Order> orderQueue;
        System.Func<Order> defaultOrderCreator;

        public bool Enabled { get; private set; }

        public Order DefaultOrder { get; private set; }

        public int Count { get { return orderQueue.Count; } }
        public Order CurrentOrder { get { return Count == 0 ? null : orderQueue.First(); } }
        public Order this[int i] { get { return orderQueue[i]; } }

        public OrderExecutor(System.Func<Order> defaultOrderCreator = null, bool enabled = true)
        {
            orderQueue = new List<Order>();
            if (defaultOrderCreator == null)
                defaultOrderCreator = () => null;
            this.defaultOrderCreator = defaultOrderCreator;
            Enabled = enabled;
        }

        public void Enable()
        {
            Enabled = true;
        }

        public void Give(Order order)
        {
            if (!Enabled)
            {
                Debug.LogWarning("Give() failed, OrderExecutor not Enabled.");
                return;
            }
            orderQueue.Add(order);
        }

        public void Cancel(int idx)
        {
            if (!Enabled)
            {
                Debug.LogWarning("Cancel() failed, OrderExecutor not Enabled.");
                return;
            }

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

        public void Cancel(Order order)
        {
            if (!Enabled)
            {
                Debug.LogWarning("Cancel() failed, OrderExecutor not Enabled.");
                return;
            }

            int idx = orderQueue.IndexOf(order);
            if (idx == -1)
                throw new System.ArgumentException(string.Format(
                    "QueueOrderExecutor does not contain {0} order to cancel.", order), "order");
            Cancel(idx);
        }

        public void CancelCurrent()
        {
            if (!Enabled)
            {
                Debug.LogWarning("Cancel() failed, OrderExecutor not Enabled.");
                return;
            }

            if (Count > 0) Cancel(0);
        }

        public void CancelAll()
        {
            if (!Enabled)
            {
                Debug.LogWarning("CancelAll() failed, OrderExecutor not Enabled.");
                return;
            }

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
            if (!Enabled)
            {
                Debug.LogWarning("Update() failed, OrderExecutor not Enabled.");
                return;
            }

            if (CurrentOrder == null)
            {
                if (DefaultOrder == null)
                    DefaultOrder = defaultOrderCreator();
                DefaultOrder.Update();
            }
            else
            {
                if (DefaultOrder != null)
                {
                    if (!DefaultOrder.Stopping)
                        DefaultOrder.Stop();
                    if (!DefaultOrder.Stopped)
                        DefaultOrder.Update();
                    if (DefaultOrder.Stopped)
                        DefaultOrder = null;
                }
                else
                {
                    CurrentOrder.Update();
                    if (CurrentOrder.Stopped)
                        orderQueue.RemoveAt(0);
                }
            }
        }

        public void Terminate()
        {
            if (!Enabled)
            {
                Debug.LogWarning("Terminate() failed, OrderExecutor not Enabled.");
                return;
            }

            foreach (var o in orderQueue)
                o.Terminate();
            orderQueue.Clear();
        }
    }
}
