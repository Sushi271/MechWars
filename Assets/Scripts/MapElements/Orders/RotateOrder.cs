using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class RotateOrder : Order
    {
        public override string Name { get { return "Rotate"; } }
        protected override bool CanStop { get { return true; } }

        float targetRotation;
        public float TargetRotation
        {
            get { return targetRotation; }
            set
            {
                if (value == targetRotation) return;
                targetRotation = value;
                CalculateDeltaAndDirection();
            }
        }
        public bool DontFinish { get; private set; }

        public float Delta { get; private set; }
        public int Direction { get; private set; }
        public float Speed { get; private set; }

        public RotateOrder(MapElement mapElement, float targetRotation = 0, bool dontFinish = false)
            : base(mapElement)
        {
            TargetRotation = targetRotation;
            DontFinish = dontFinish;
        }

        protected override void OnStart()
        {
            Stat speedStat = null;
            TryFail(OrderResultAsserts.AssertMapElementHasStat(MapElement, StatNames.RotationSpeed, out speedStat));
            if (Failed) return;

            CalculateDeltaAndDirection();
            Speed = speedStat.Value;
        }

        void CalculateDeltaAndDirection()
        {
            Delta = (TargetRotation - MapElement.Rotation).NormalizeAngle();
            Direction = System.Math.Sign(Delta);
        }

        protected override void OnUpdate()
        {
            if (State == OrderState.Stopping) return;

            float dDist = Speed * 360 * Time.deltaTime;
            if (dDist < Mathf.Abs(Delta))
            {
                MapElement.Rotation += Direction * dDist;
                CalculateDeltaAndDirection();
            }
            else
            {
                MapElement.Rotation = TargetRotation;
                if (!DontFinish) Succeed();
            }
        }
        
        protected override string SpecificsToString()
        {
            return TargetRotation.ToString();
        }
    }
}
