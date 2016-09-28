using MechWars.MapElements.Orders;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Move
    {
        SingleMoveOrder singleMoveOrder;

        public Unit Unit { get { return singleMoveOrder.Unit; } }
        public IVector2 Destination { get { return singleMoveOrder.Destination; } }
        public float Speed { get { return singleMoveOrder.Speed; } }

        public Vector2 Delta { get; private set; }
        public Vector2 Direction { get; private set; }
        public bool Done { get; private set; }

        public Vector2 Velocity { get { return Direction * Speed; } }

        public Move(SingleMoveOrder singleMoveOrder)
        {
            this.singleMoveOrder = singleMoveOrder;
            
            Delta = Destination - Unit.Coords;
            Direction = Delta.normalized;
        }

        public void Update()
        {
            Delta = Destination - Unit.Coords;
            float dDist = Speed * Time.deltaTime;
            if (dDist < Delta.magnitude)
                Unit.Coords += Direction * dDist;
            else
            {
                Unit.Coords = Destination;
                Done = true;
            }
        }
    }
}
