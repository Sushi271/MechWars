using UnityEngine;
using System.Collections;
using MechWars.Orders;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        public Army army;

        public float speed = 5;

        OrderExecutor orderExecutor;
        public IOrder CurrentOrder { get { return orderExecutor.CurrentOrder; } }

        public Unit()
        {
            selectable = true;
            orderExecutor = new OrderExecutor(this);
        }

        protected override void OnUpdate()
        {
            orderExecutor.Update();
        }

        public bool MoveStepTo(int x, int y)
        {
            // TODO: check whether space is occupied
            float dx = x - X;
            float dy = y - Y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            float deltaDist = speed * Time.deltaTime;

            if (deltaDist >= dist)
            {
                X = x;
                Y = y;
                return true;
            }

            var dPos = new Vector3(dx, 0, dy).normalized * deltaDist;
            transform.position += dPos;
            return false;
        }

        public void GiveOrder(Order order)
        {
            // TODO: provide control over order type
            orderExecutor.Give(order);
        }
    }
}