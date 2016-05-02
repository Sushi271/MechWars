using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class IdleOrder : Order<MapElement>
    {
        AttackOrder attack;
        MapElement autoAttackTarget;
        bool targetLost;
        
        public IdleOrder(Unit orderedUnit)
            : base("Idle", orderedUnit)
        {
        }

        public IdleOrder(Building orderedBuilding)
            : base("Idle", orderedBuilding)
        {
        }

        protected override bool RegularUpdate()
        {
            if (MapElement.canAttack)
            {
                if (attack == null)
                {
                    autoAttackTarget = MapElement.AcquireTarget();
                    if (autoAttackTarget != null)
                    {
                        if (MapElement is Unit)
                            attack = new AttackOrder((Unit)MapElement, autoAttackTarget);
                        else if (MapElement is Building)
                            attack = new AttackOrder((Building)MapElement, autoAttackTarget);
                        else throw new System.Exception("MapElement is neither Unit nor Building.");

                        targetLost = false;
                    }
                }
                else if (targetLost || !autoAttackTarget.Alive
                    || !MapElement.MapElementInRange(autoAttackTarget))
                {
                    targetLost = true;
                    if (!attack.Stopping)
                        attack.Stop();
                    if (attack.Stopped)
                    {
                        attack = null;
                        autoAttackTarget = null;
                    }
                }
            }

            if (attack != null)
                attack.Update();
            else CasualIdleBehaviour();

            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (autoAttackTarget != null)
            {
                if (!attack.AttackingInProgress) return true;

                attack.Update();
                if (!attack.Stopped) return false;
                else
                {
                    attack = null;
                    autoAttackTarget = null;
                    return true;
                }
            }
            else
            {
                // Finish CasualIdleBehaviour()
                return true;
            }
        }

        protected override void TerminateCore()
        {
        }

        protected override void OnStopCalled()
        {
            if (attack != null)
                attack.Stop();
        }

        float minRotationTime = 5;
        float maxRotationTime = 15;
        float nextRotationTime = -1;
        float timeToRotation = 0;

        bool rotating;
        float fullRotationDuration = 0.5f;
        float rotationDuration;
        float toRotate;
        float rotationProgress;
        float startRotation;

        void CasualIdleBehaviour()
        {
            if (MapElement.canRotate)
            {
                if (!rotating)
                {
                    if (nextRotationTime == -1)
                        nextRotationTime = Random.Range(minRotationTime, maxRotationTime);
                    timeToRotation += Time.deltaTime;
                    if (timeToRotation > nextRotationTime)
                    {
                        timeToRotation -= nextRotationTime;
                        nextRotationTime = -1;
                        toRotate = Random.Range(-180, 180);
                        rotationDuration = fullRotationDuration * Mathf.Abs(toRotate) / 360;
                        startRotation = MapElement.transform.rotation.eulerAngles.y;
                        rotating = true;
                        rotationProgress = 0;
                    }
                }
                else
                {

                    var dProgress = Time.deltaTime / rotationDuration;
                    rotationProgress += dProgress;
                    if (rotationProgress > 1)
                    {
                        rotating = false;
                        rotationProgress = 1;
                    }
                    var rotation = startRotation + rotationProgress * toRotate;
                    MapElement.transform.localRotation = Quaternion.Euler(0, rotation, 0);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Idle");
        }
    }
}
