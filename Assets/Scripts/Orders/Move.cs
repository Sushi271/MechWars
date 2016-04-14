using System.Collections.Generic;
using MechWars.MapElements;
using MechWars.Pathfinding;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.Orders
{
    public class Move : Order
    {
        public IVector2 Destination { get; private set; }

        Path path;
        bool pathNeedsUpdate;

        public Move(Unit orderedUnit, IVector2 destination)
            : base("Move", orderedUnit)
        {
            Destination = destination;
            path = null;
            pathNeedsUpdate = true;
        }

        protected override bool RegularUpdate()
        {
            if (pathNeedsUpdate)
                CalculatePath();
            if (SingleMove())
            {
                path.Pop();
                if (path.Length == 0) return true;
                pathNeedsUpdate = true;
            }
            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (pathNeedsUpdate) return true;
            if (SingleMove()) return true;
            return false;
        }

        void CalculatePath()
        {
            path = new AStarPathfinder().FindPath(
                new CoordPair(Unit.Coords.Round()),
                new CoordPair(Destination));
            pathNeedsUpdate = false;
        }

        bool SingleMove()
        {
            if (path.First == null) return true;
            if (path.First.Next == null) return true;
            var coords = path.First.Next.Coords;

            var frm = Globals.FieldReservationMap;
            var vec = coords.Vector;
            var occupier = frm[vec];
            
            if (occupier != Unit)
            {
                if (occupier != null)
                    throw new System.Exception(string.Format("Unit {0} cannot move to field {1} - it's occupied.",
                        Unit.ToString(), vec.ToString()));

                frm.MakeReservation(Unit, vec);
                frm.ReleaseReservation(Unit, Unit.Coords.Round());
            }
            
            bool finished;
            Unit.MoveStepTo(coords.X, coords.Y, out finished);
            return finished;
        }

        //bool IsDestinationReached()
        //{
        //    return Unit.Coords == Destination;
        //}

        public override string ToString()
        {
            return string.Format("Move [ {0}, {1} ]", Destination.X, Destination.Y);
        }

        class MoveDetails
        {
        }
    }
}
