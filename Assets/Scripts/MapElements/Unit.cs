﻿using MechWars.MapElements.Orders;
using MechWars.MapElements.Orders.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        public bool canCollect;
        public List<OrderAction<Unit>> orderActions;

        public SingleOrderExecutor OrderExecutor { get; private set; }

        protected override bool CanAddToArmy { get { return true; } }
        public override bool Selectable { get { return true; } }
        public override bool CanAttack { get { return orderActions.Any(oa => oa.IsAttack); } }
        public override bool CanBeAttacked { get { return true; } }
        public virtual bool CanBeEscorted { get { return true; } }

        public Unit()
        {
            OrderExecutor = new SingleOrderExecutor(() => new IdleOrder(this));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isShadow) return;

            OrderExecutor.Update();
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