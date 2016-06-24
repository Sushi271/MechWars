using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class OrderQueue
    {
        List<Order> queue;
        System.Func<Order> defaultOrderCreator;

        public bool Enabled { get; private set; }
        public bool GiveReplaces { get; set; }

        public Order DefaultOrder { get; private set; }

        public int OrderCount { get { return queue.Count; } }
        public Order CurrentOrder { get { return OrderCount == 0 ? null : queue.First(); } }
        public Order this[int i] { get { return queue[i]; } }

        public OrderQueue(System.Func<Order> defaultOrderCreator = null, bool enabled = true)
        {
            queue = new List<Order>();
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
            queue.Add(order);
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
                queue[idx].CancelBrandNew();
                queue.RemoveAt(idx);
            }
            else if (!CurrentOrder.InFinalState && CurrentOrder.State != OrderState.Stopping)
                CurrentOrder.Stop();
        }

        public void Cancel(Order order)
        {
            if (!Enabled)
            {
                LogNotEnabledWarning("Cancel");
                return;
            }

            int idx = queue.IndexOf(order);
            if (idx == -1)
                throw new System.ArgumentException(string.Format(
                    "OrderQueue does not contain {0} order to cancel.", order), "order");
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
                queue[OrderCount - 1].CancelBrandNew();
                queue.RemoveAt(OrderCount - 1);
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
                {
                    DefaultOrder = defaultOrderCreator();
                    DefaultOrder.Start();
                }
                if (DefaultOrder != null && DefaultOrder.CanUpdate)
                    DefaultOrder.Update();
            }
            else
            {
                if (DefaultOrder != null)
                {
                    if (DefaultOrder.State == OrderState.Started)
                        DefaultOrder.Stop();
                    if (!DefaultOrder.InFinalState)
                        DefaultOrder.Update();
                    if (DefaultOrder.InFinalState)
                        DefaultOrder = null;
                }
                else
                {
                    if (CurrentOrder.State == OrderState.BrandNew)
                        CurrentOrder.Start();
                    CurrentOrder.Update();
                    if (CurrentOrder.InFinalState)
                        queue.RemoveAt(0);
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

            foreach (var o in queue)
                o.Terminate();
            queue.Clear();
        }

        void LogNotEnabledWarning(string funcName)
        {
            Debug.LogWarning(string.Format("{0}() failed, OrderQueue not Enabled.", funcName));
        }
    }
}
