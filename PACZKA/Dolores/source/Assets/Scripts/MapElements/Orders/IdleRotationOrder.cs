using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class IdleRotationOrder : ComplexOrder
    {
        static float attackHeadRotationProbability = 0.8f;

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

            IRotatable rotatedObject = null;
            var head = MapElement.attackHead as HorizontalAttackHead;
            if (MapElement.CanRotateItself && head != null)
            {
                var r = Random.Range(0f, 1f);
                if (r <= attackHeadRotationProbability)
                    rotatedObject = head;
                else rotatedObject = MapElement;
            }
            else if (MapElement.CanRotateItself)
                rotatedObject = MapElement;
            else if (head != null)
                rotatedObject = head;

            if (rotatedObject != null)
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

                    rotateOrder = new RotateOrder(MapElement, true, targetRotation);
                    GiveSubOrder(rotateOrder);
                }
            }
        }
    }
}
