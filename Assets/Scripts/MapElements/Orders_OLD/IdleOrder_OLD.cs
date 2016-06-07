﻿using MechWars.MapElements.Jobs;
using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders_OLD
{
    public class IdleOrder_OLD : Order_OLD
    {
        AttackOrder_OLD attack;
        MapElement autoAttackTarget;
        bool targetLost;

        public IdleOrder_OLD(MapElement orderedMapElement)
            : base("Idle", orderedMapElement)
        {
        }        

        protected override bool RegularUpdate()
        {
            if (MapElement.CanAttack)
            {
                if (attack == null)
                {
                    autoAttackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
                    if (autoAttackTarget != null)
                    {
                        if (MapElement is Unit)
                            attack = new AttackOrder_OLD((Unit)MapElement, autoAttackTarget);
                        else if (MapElement is Building)
                            attack = new AttackOrder_OLD((Building)MapElement, autoAttackTarget);
                        else throw new System.Exception("MapElement is neither Unit nor Building.");

                        targetLost = false;
                    }
                }
                else if (targetLost || !autoAttackTarget.Alive
                    || !MapElement.HasMapElementInRange(autoAttackTarget, StatNames.AttackRange))
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

        void CasualIdleBehaviour()
        {
            if (MapElement.canRotate &&
                MapElement.JobQueue.Empty)
            {
                if (nextRotationTime == -1)
                    nextRotationTime = Random.Range(minRotationTime, maxRotationTime);
                timeToRotation += Time.deltaTime;
                if (timeToRotation > nextRotationTime)
                {
                    timeToRotation -= nextRotationTime;
                    nextRotationTime = -1;
                    var angleToRotate = Random.Range(-180, 180);
                    var targetRotation = MapElement.Rotation + angleToRotate;
                    MapElement.JobQueue.Add(new RotateJob(MapElement, targetRotation));
                }
            }
        }
    }
}
