using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class OrderExecutor
    {
        List<Order> orderQueue;
        System.Func<Order> defaultOrderCreator;

        public bool Enabled { get; private set; }
        public bool GiveReplaces { get; set; }

        public Order DefaultOrder { get; private set; }

        public int OrderCount { get { return orderQueue.Count; } }
        public Order CurrentOrder { get { return OrderCount == 0 ? null : orderQueue.First(); } }
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

        public void Give(Order order, bool forceAdd = false)
        {
            if (!Enabled)
            {
                LogNotEnabledWarning("Give");
                return;
            }
            if (OrderCount > 0 && GiveReplaces && !forceAdd)
                CancelAll();
            orderQueue.Add(order);
        }

        public void Cancel(int idx)
        {
            if (!Enabled)
            {
                LogNotEnabledWarning("Cancel");
                return;
            }

            if (idx < 0 || OrderCount <= idx)
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
                LogNotEnabledWarning("Cancel");
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
                LogNotEnabledWarning("CancelCurrent");
                return;
            }

            if (OrderCount > 0) Cancel(0);
        }

        public void CancelAll()
        {
            if (!Enabled)
            {
                LogNotEnabledWarning("CancelAll");
                return;
            }

            if (OrderCount == 0) return;
            while (OrderCount > 1)
            {
                orderQueue[OrderCount - 1].Terminate();
                orderQueue.RemoveAt(OrderCount - 1);
            }
            CancelCurrent();
        }

        public void Update()
        {
            if (!Enabled)
            {
                LogNotEnabledWarning("Update");
                return;
            }

            if (CurrentOrder == null)
            {
                if (DefaultOrder == null)
                    DefaultOrder = defaultOrderCreator();
                if (DefaultOrder != null)
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
                LogNotEnabledWarning("Terminate");
                return;
            }

            foreach (var o in orderQueue)
                o.Terminate();
            orderQueue.Clear();
        }

        void LogNotEnabledWarning(string funcName)
        {
            Debug.LogWarning(string.Format("{0}() failed, OrderExecutor not Enabled.", funcName));
        }
    }
}
