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

        public Move(List<Unit> orderedUnits, IVector2 destination)
            : base("Move", orderedUnits)
        {
            Destination = destination;

            pathNeedsUpdate = true;
        }

        protected override bool RegularUpdate(Unit unit)
        {
            if (pathNeedsUpdate)
                CalculatePath(unit);
            if (SingleMove(unit))
            {
                path.Pop();
                if (IsDestinationReached(unit))
                    return true;
                //pathNeedsUpdate = true;
            }
            return false;
        }

        protected override bool StoppingUpdate(Unit unit)
        {
            if (pathNeedsUpdate) return true;
            if (SingleMove(unit)) return true;
            return false;
        }

        void CalculatePath(Unit unit)
        {
            path = new AStarPathfinder().FindPath(
                new CoordPair(unit.Coords.Round()),
                new CoordPair(Destination));
            pathNeedsUpdate = false;
        }

        bool SingleMove(Unit unit)
        {
            if (path.First == null) return true;
            if (path.First.Next == null) return true;
            var coords = path.First.Next.Coords;

            return unit.MoveStepTo(coords.X, coords.Y);
        }

        bool IsDestinationReached(Unit unit)
        {
            return unit.Coords == Destination;
        }

        public override string ToString()
        {
            return string.Format("Move [ {0}, {1} ]", Destination.X, Destination.Y);
        }
    }
}
