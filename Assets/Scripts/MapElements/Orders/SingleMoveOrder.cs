using MechWars.MapElements.Statistics;
using MechWars.Utils;

namespace MechWars.MapElements.Orders
{
    public class SingleMoveOrder : Order
    {
        public override string Name { get { return "SingleMove"; } }

        public Unit Unit { get; private set; }
        public IVector2 Destination { get; private set; }
        public float Speed { get; private set; }

        public SingleMoveOrder(Unit unit, IVector2 destination)
            : base(unit)
        {
            Unit = unit;
            Destination = destination;
        }

        public bool LastMinuteChangeDestination(IVector2 destination)
        {
            if (State != OrderState.BrandNew) return false;
            Destination = destination;
            return true;
        }

        protected override void OnStart()
        {
            Stat speedStat = null;

            TryFail(OrderResultAsserts.AssertMapElementIsInIntegerCoords(MapElement));
            TryFail(OrderResultAsserts.AssertDestinationIsNeighbourCoords(MapElement, Destination));
            TryFail(OrderResultAsserts.AssertDestinationIsInsideMap(Destination));
            TryFail(OrderResultAsserts.AssertDestinationIsNotOccupied(MapElement, Destination));
            TryFail(OrderResultAsserts.AssertMapElementHasStat(MapElement, StatNames.MovementSpeed, out speedStat));
            if (Failed) return;
            
            Speed = speedStat.Value;

            Globals.FieldReservationMap.MakeReservation(MapElement, Destination);
            Globals.FieldReservationMap.ReleaseReservation(MapElement, MapElement.Coords.Round());

            Unit.SetMove(this);
        }

        protected override void OnUpdate()
        {
            if (Unit.Move == null)
                Succeed();
        }
        
        protected override string SpecificsToString()
        {
            return Destination.ToString();
        }
    }
}
