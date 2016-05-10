using MechWars.MapElements.Jobs;
using MechWars.Pathfinding;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class MoveOrder : Order<Unit>
    {
        Path path;
        bool pathNeedsUpdate;

        bool dontStop;

        public Unit Unit { get; private set; }

        IVector2 destination;
        public IVector2 Destination
        {
            get { return destination; }
            set
            {
                if (destination != value)
                    pathNeedsUpdate = true;
                destination = value;
            }
        }

        public bool SingleMoveInProgress { get; private set; }

        public event System.Action OnSingleMoveFinished;

        public MoveOrder(Unit orderedUnit, IVector2 destination, bool dontStop = false)
            : base("Move", orderedUnit)
        {
            Unit = (Unit)MapElement;
            Destination = destination;

            this.dontStop = dontStop;

            path = null;
            pathNeedsUpdate = true;
        }

        protected override bool RegularUpdate()
        {
            if (pathNeedsUpdate)
                CalculatePath();
            if (path.Count > 0)
            {
                if (SingleMove())
                {
                    SingleMoveInProgress = false;
                    if (OnSingleMoveFinished != null)
                        OnSingleMoveFinished.Invoke();
                    path.Pop();
                    if (path.Length == 0 || Unit.Coords == path.Last.Coords.Vector)
                        return !dontStop;
                    pathNeedsUpdate = true;
                }
            }
            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (pathNeedsUpdate) return true;
            if (SingleMove()) return true;
            return false;
        }

        protected override void TerminateCore()
        {
        }

        void CalculatePath()
        {
            path = new AStarPathfinder().FindPath(
                new CoordPair(Unit.Coords.Round()),
                new CoordPair(Destination),
                Unit);
            pathNeedsUpdate = false;
        }

        bool SingleMove()
        {
            SingleMoveInProgress = true;

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
                
                var delta = vec - Unit.Coords.Round();
                var angle = -UnityExtensions.AngleFromTo(Vector2.up, delta);                

                Unit.JobQueue.Add(new RotateJob(Unit, angle));
                Unit.JobQueue.Add(new MoveJob(Unit, delta));
            }

            return Unit.JobQueue.Empty;
        }

        protected override string SpecificsToString()
        {
            return string.Format("{0}, {1}", Destination.X, Destination.Y);
        }
    }
}
