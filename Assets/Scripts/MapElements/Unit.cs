using UnityEngine;
using System.Collections;
using MechWars.Orders;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        OrderExecutor orderExecutor;
        public IOrder CurrentOrder { get { return orderExecutor.CurrentOrder; } }

        public Unit()
        {
            selectable = true;
            orderExecutor = new OrderExecutor(this);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            orderExecutor.Update();

            //DEBUG
            if (Selected)
            {
                var hitPoints = Stats[StatNames.HitPoints];
                if (Input.GetKeyDown(KeyCode.KeypadPlus))
                    hitPoints.Value += 10;
                if (Input.GetKeyDown(KeyCode.KeypadMinus))
                    hitPoints.Value -= 10;
            }
        }

        public void MoveStepTo(int x, int y, out bool finished)
        {
            float dx = x - X;
            float dy = y - Y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            var speed = Stats[StatNames.Speed];
            if (speed == null)
                throw new System.Exception(string.Format("Missing {0} attribute in Unit's Stats! (Unit {1})",
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

        public void GiveOrder(Order order)
        {
            // TODO: provide control over order type
            orderExecutor.Give(order);
        }
    }
}