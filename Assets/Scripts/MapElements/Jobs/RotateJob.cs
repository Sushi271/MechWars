using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Jobs
{
    public class RotateJob : Job
    {
        public Unit Unit { get; private set; }
        public float TargetRotation { get; private set; }

        float startRotation;
        float angleToRotate;
        float rotationDuration;
        float rotationProgress;

        public RotateJob(MapElement mapElement, float targetRotation)
            : base(mapElement)
        {
            if (!MapElement.canRotate)
            {
                SetDone();
                return;
            }

            TargetRotation = targetRotation;

            startRotation = mapElement.Rotation;
            angleToRotate = (TargetRotation - startRotation).NormalizeAngle();

            var rotationSpeedStat = MapElement.Stats[StatNames.RotationSpeed];
            if (rotationSpeedStat == null)
                throw new System.Exception(string.Format("Missing {0} Stat in MapElement's Stats! (MapElement {1})",
                    StatNames.RotationSpeed, MapElement));
            var rotationSpeed = rotationSpeedStat.Value;
            rotationDuration = Mathf.Abs(angleToRotate) / (rotationSpeed * 360);
            rotationProgress = 0;
        }

        protected override void OnUpdate()
        {
            var dProgress = Time.deltaTime / rotationDuration;
            rotationProgress += dProgress;
            if (rotationProgress > 1)
            {
                rotationProgress = 1;
                SetDone();
            }
            var rotation = startRotation + rotationProgress * angleToRotate;
            MapElement.Rotation = rotation;
        }
    }
}