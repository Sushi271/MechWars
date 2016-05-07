using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Jobs
{
    public class MoveJob : Job
    {
        public Unit Unit { get; private set; }
        public IVector2 Delta { get; private set; }
        public IVector2 Destination { get; private set; }

        public MoveJob(Unit unit, IVector2 direction)
            : base(unit)
        {
            Unit = unit;
            Delta = direction.Sign();
            Destination = unit.Coords.Round() + Delta;
        }

        public override void OnUpdate()
        {
            var speed = Unit.Stats[StatNames.Speed];
            if (speed == null)
                throw new System.Exception(string.Format("Missing {0} Stat in Unit's Stats! (Unit {1})",
                    StatNames.Speed, Unit));

            var toDest = Destination - Unit.Coords;
            float dDist = speed.Value * Time.deltaTime;
            if (dDist < toDest.magnitude)
                Unit.Coords += Delta.Normalized * dDist;
            else
            {
                Unit.Coords = Destination;
                SetDone();
            }
        }
    }
}
