using MechWars.Pathfinding;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class MoveOrder : ComplexOrder
    {
        IVector2 destination;

        Path path;
        bool pathNeedsUpdate;

        RotateOrder rotateOrder;
        SingleMoveOrder singleMoveOrder;

        public override string Name { get { return "Move"; } }

        public Unit Unit { get; private set; }
        public IVector2 Destination
        {
            get { return destination; }
            set
            {
                if (destination == value) return;
                destination = value;
                pathNeedsUpdate = true;
            }
        }
        public bool DontFinish { get; private set; }

        public MoveOrder(Unit unit, IVector2 destination, bool dontFinish = false)
            : base(unit)
        {
            Unit = unit;
            Destination = destination;
            DontFinish = dontFinish;
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertMapElementIsInIntegerCoords(MapElement));
        }

        protected override void OnUpdate()
        {
            if (HasSubOrder || State == OrderState.Stopping) return;

            if (pathNeedsUpdate || path == null)
                CalculatePath();

            if (!path.Empty)
            {
                var singleMoveDestination = path.First.Next.Coords.Vector;
                var delta = singleMoveDestination - MapElement.Coords;
                var angle = UnityExtensions.AngleFromToXZ(Vector2.up, delta);

                rotateOrder = new RotateOrder(Unit, false, angle);
                GiveSubOrder(rotateOrder);
            }
            else if (!DontFinish) Succeed();
        }

        protected override void OnSubOrderStarting()
        {
            if (SubOrder == singleMoveOrder)
            {
                if (Globals.Map.FieldOccupiedFor(MapElement, singleMoveOrder.Destination))
                {
                    CalculatePath();
                    singleMoveOrder.LastMinuteChangeDestination(path.First.Next.Coords.Vector);
                }
            }
        }

        protected override void OnSubOrderFinished()
        {
            if (SubOrder == rotateOrder)
            {
                rotateOrder = null;
                singleMoveOrder = new SingleMoveOrder(Unit, path.First.Next.Coords.Vector);
                GiveSubOrder(singleMoveOrder);
            }
            else if (SubOrder == singleMoveOrder)
            {
                singleMoveOrder = null;
                path.Pop();
                if (!path.Empty)
                    pathNeedsUpdate = true;
                else if (!DontFinish) Succeed();
            }
        }

        void CalculatePath()
        {
            path = new AStarPathfinder().FindPath(
                new CoordPair(MapElement.Coords.Round()),
                new CoordPair(Destination), MapElement);
            pathNeedsUpdate = false;
        }

        protected override string SpecificsToStringCore()
        {
            return Destination.ToString();
        }
    }
}
