using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class RotateOrder : Order
    {
        IRotatable rotatedObject;
        string speedStatName;

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

        bool horizontalDone;

        // [MESS] Vertical
        public bool RotatesVertically { get; set; }

        float targetHeadPitch;
        public float TargetHeadPitch
        {
            get { return targetHeadPitch; }
            set
            {
                if (value == targetHeadPitch) return;
                targetHeadPitch = value;
                CalculateHeadPitchDeltaAndDirection();
            }
        }

        public float HeadPitchDelta { get; private set; }
        public int HeadPitchDirection { get; private set; }
        public float HeadPitchSpeed { get; private set; }

        bool verticalDone;
        // [/MESS]

        public RotateOrder(MapElement mapElement, bool tryRotateHead = false, float targetRotation = 0, bool dontFinish = false)
            : base(mapElement)
        {
            var head = mapElement.attackHead as HorizontalAttackHead;
            if (tryRotateHead && head != null)
            {
                rotatedObject = head;
                speedStatName = StatNames.AttackHeadRotationSpeed;
            }
            else
            {
                rotatedObject = mapElement;
                speedStatName = StatNames.RotationSpeed;
            }

            TargetRotation = targetRotation;
            DontFinish = dontFinish;
        }

        protected override void OnStart()
        {
            Stat speedStat = null;
            TryFail(OrderResultAsserts.AssertMapElementHasStat(MapElement, speedStatName, out speedStat));
            if (Failed) return;

            Stat headPitchSpeedStat = null;
            if (RotatesVertically)
            {
                TryFail(OrderResultAsserts.AssertMapElementHasStat(MapElement, StatNames.AttackHeadPitchSpeed, out headPitchSpeedStat));
                if (Failed) return;
            }

            CalculateDeltaAndDirection();
            CalculateHeadPitchDeltaAndDirection();

            Speed = speedStat.Value;
            if (RotatesVertically)
                HeadPitchSpeed = headPitchSpeedStat.Value;
        }

        void CalculateDeltaAndDirection()
        {
            Delta = (TargetRotation - rotatedObject.Rotation).NormalizeAngle();
            Direction = System.Math.Sign(Delta);
        }

        void CalculateHeadPitchDeltaAndDirection()
        {
            HeadPitchDelta = (TargetHeadPitch - rotatedObject.HeadPitch).NormalizeAngle();
            HeadPitchDirection = System.Math.Sign(HeadPitchDelta);
        }

        protected override void OnUpdate()
        {
            if (State == OrderState.Stopping) return;

            if (!horizontalDone)
            {
                float dDist = Speed * 360 * Time.deltaTime;
                if (dDist < Mathf.Abs(Delta))
                {
                    rotatedObject.Rotation += Direction * dDist;
                    CalculateDeltaAndDirection();
                }
                else
                {
                    rotatedObject.Rotation = TargetRotation;
                    if (!DontFinish)
                        horizontalDone = true;
                }
            }

            if (RotatesVertically && !verticalDone)
            {
                float dDist = HeadPitchSpeed * 360 * Time.deltaTime;
                if (dDist < Mathf.Abs(HeadPitchDelta))
                {
                    rotatedObject.HeadPitch += HeadPitchDirection * dDist;
                    CalculateHeadPitchDeltaAndDirection();
                }
                else
                {
                    rotatedObject.HeadPitch = TargetHeadPitch;
                    verticalDone = true;
                }
            }

            if (horizontalDone && (!RotatesVertically || verticalDone))
                Succeed();
        }
        
        protected override string SpecificsToString()
        {
            return TargetRotation.ToString();
        }
    }
}
