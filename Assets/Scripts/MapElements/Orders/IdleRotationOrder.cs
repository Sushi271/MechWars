using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class IdleRotationOrder : ComplexOrder
    {
        float minRotationTime = 5;
        float maxRotationTime = 15;
        float nextRotationTime = -1;
        float timeToRotation = 0;
        float minAngleToRotate = 45;

        RotateOrder rotateOrder;

        public override string Name { get { return "IdleRotation"; } }

        public IdleRotationOrder(MapElement mapElement)
            : base(mapElement)
        {
        }

        protected override void OnUpdate()
        {
            if (HasSubOrder || State == OrderState.Stopping) return;

            if (MapElement.canRotate)
            {
                if (nextRotationTime == -1)
                    nextRotationTime = Random.Range(minRotationTime, maxRotationTime);
                timeToRotation += Time.deltaTime;
                if (timeToRotation > nextRotationTime)
                {
                    timeToRotation = 0;
                    nextRotationTime = -1;
                    var angleToRotate = Random.Range(minAngleToRotate, 180) * Random2.Sign();
                    var targetRotation = MapElement.Rotation + angleToRotate;
                    rotateOrder = new RotateOrder(MapElement, targetRotation);
                    GiveSubOrder(rotateOrder);
                }
            }
        }
    }
}
