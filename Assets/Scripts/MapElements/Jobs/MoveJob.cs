using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Jobs
{
    public class MoveJob : Job
    {
        public Unit Unit { get; private set; }
        public IVector2 Delta { get; private set; }
        public Vector2 Direction { get; private set; }
        public IVector2 Destination { get; private set; }
        
        public float Speed { get; private set; }
        public Vector2 Velocity { get; private set; }

        public MoveJob(Unit unit, IVector2 delta)
            : base(unit)
        {
            Unit = unit;
            Delta = delta.Sign();
            Direction = Delta.Normalized;
            Destination = unit.Coords.Round() + Delta;

            var speedStat = Unit.Stats[StatNames.MovementSpeed];
            if (speedStat == null)
                throw new System.Exception(string.Format("Missing {0} Stat in Unit's Stats! (Unit {1})",
                    StatNames.MovementSpeed, Unit));
            Speed = speedStat.Value;
            Velocity = Delta.Normalized * Speed;
        }

        protected override void OnUpdate()
        {
            var toDest = Destination - Unit.Coords;
            float dDist = Speed * Time.deltaTime;
            if (dDist < toDest.magnitude)
                Unit.Coords += Direction * dDist;
            else
            {
                Unit.Coords = Destination;
                SetDone();
            }
        }
    }
}
