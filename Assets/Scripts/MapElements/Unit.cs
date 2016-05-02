﻿using MechWars.MapElements.Orders;
using MechWars.MapElements.Statistics;
using System.Text;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        public bool canCollect;

        public SingleOrderExecutor OrderExecutor { get; private set; }

        public Unit()
        {
            selectable = true;
            OrderExecutor = new SingleOrderExecutor(() => new IdleOrder(this));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isShadow) return;

            OrderExecutor.Update();
        }

        public void MoveStepTo(int x, int y, out bool finished)
        {
            float dx = x - X;
            float dy = y - Y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            var speed = Stats[StatNames.Speed];
            if (speed == null)
                throw new System.Exception(string.Format("Missing {0} Stat in Unit's Stats! (Unit {1})",
                    StatNames.Speed, this));
            float deltaDist = speed.Value * Time.deltaTime;

            if (deltaDist >= dist)
            {
                X = x;
                Y = y;
                finished = true;
                return;
            }

            var dPos = new Vector3(dx, 0, dy).normalized * deltaDist;
            transform.position += dPos;
            finished = false;
        }

        public void GiveOrder(IOrder order)
        {
            if (order is Order<Unit> || order is Order<MapElement>)
                OrderExecutor.Give(order);
            else throw new System.Exception(string.Format(
                "Order {0} not suitable for MapElement {1}.", order, this));
        }

        protected override void OnLifeEnd()
        {
            base.OnLifeEnd();
            OrderExecutor.Terminate();
        }
        
        public override StringBuilder TEMP_PrintStatus()
        {
            return base.TEMP_PrintStatus().AppendLine()
                .AppendLine(string.Format("Can collect resources: {0}", canCollect))
                .Append(string.Format("Current order: {0}", OrderExecutor.CurrentOrder == null ? "NONE" : OrderExecutor.CurrentOrder.ToString()));
        }
    }
}