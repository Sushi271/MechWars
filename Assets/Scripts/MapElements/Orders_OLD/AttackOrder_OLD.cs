using MechWars.MapElements.Attacks;
using MechWars.MapElements.Jobs;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders_OLD
{
    public class AttackOrder_OLD : Order_OLD
    {
        public static AttackOrder_OLD Create(MapElement orderedMapElement, MapElement target)
        {
            if (orderedMapElement is Unit)
                return new AttackOrder_OLD((Unit)orderedMapElement, target);
            if (orderedMapElement is Building)
                return new AttackOrder_OLD((Building)orderedMapElement, target);
            throw new System.ArgumentException("\"MapElement orderedMapElement\" argument must be either Unit or Building.");
        }

        Attack attack;
        AttackJob attackJob;

        public MapElement Target { get; private set; }
        public bool AttackingInProgress { get { return attack != null; } }

        public AttackOrder_OLD(Unit orderedUnit, MapElement target)
            : base("Attack", orderedUnit)
        {
            AttackOrderHelper.AssertTargetsCanBeAttacked(target.AsEnumerable());
            Target = target;
        }

        public AttackOrder_OLD(Building orderedBuilding, MapElement target)
            : base("Attack", orderedBuilding)
        {
            AttackOrderHelper.AssertTargetsCanBeAttacked(target.AsEnumerable());
            Target = target;
        }

        protected override bool RegularUpdate()
        {
            return CommonUpdate();
        }

        protected override bool StoppingUpdate()
        {
            return CommonUpdate();
        }

        protected override void TerminateCore()
        {
        }

        bool CommonUpdate()
        {
            if (!MapElement.CanAttack)
                throw new System.Exception(string.Format(
                    "Order {0} called for MapElement {1}, but it cannot attack.", Name, MapElement));

            bool stopped;
            MakeAttack(out stopped);
            return stopped;
        }

        float cooldown = 0;

        void MakeAttack(out bool stopped)
        {
            stopped = false;

            if (!AttackingInProgress)
            {
                if (Stopping || !Target.Alive)
                {
                    stopped = true;
                    return;
                }

                cooldown -= Time.deltaTime;
                if (cooldown <= 0)
                {
                    var aim = Target.GetClosestFieldTo(MapElement.Coords);
                    if (MapElement.HasPositionInRange(aim, StatNames.AttackRange))
                        StartAttack(aim);
                }
            }
        }

        void StartAttack(Vector2 aim)
        {
            attack = PickAttack();
            if (attack != null)
            {
                attackJob = new AttackJob(attack, MapElement, Target, aim);
                var angle = -UnityExtensions.AngleFromToXZ(Vector2.up, attackJob.Direction);
                var rotateJob = new RotateJob(MapElement, angle);
                rotateJob.OnJobDone += RotateJob_OnJobDone;
                attackJob.OnJobDone += AttackJob_OnJobDone;

                MapElement.JobQueue.Add(rotateJob);
                MapElement.JobQueue.Add(attackJob);
            }
        }
        
        private void RotateJob_OnJobDone()
        {
            bool cancel = !Target.Alive;
            if (!cancel)
            {
                var aim = Target.GetClosestFieldTo(MapElement.Coords);
                cancel = aim != attackJob.Aim;
            }
            if (cancel) CancelAttack();
        }

        private void AttackJob_OnJobDone()
        {
            var attackSpeedStat = MapElement.Stats[StatNames.AttackSpeed];
            float attackSpeed = 1;
            if (attackSpeedStat != null && attackSpeedStat.Value > 0)
                attackSpeed = attackSpeedStat.Value;
            float attackInterval = 1 / attackSpeed;
            cooldown += attackInterval;

            CancelAttack();
        }

        void CancelAttack()
        {
            attack = null;
            attackJob = null;
            MapElement.JobQueue.Clear();
        }

        Attack PickAttack()
        {
            var attacks = MapElement.GetComponents<Attack>();
            if (attacks.Length == 0)
            {
                Debug.LogWarning(string.Format("MapElement {0} has no Attacks.", MapElement));
                return null;
            }
            int idx = Random.Range(0, attacks.Length);
            return attacks[idx];
        }

        protected override string SpecificsToString()
        {
            return string.Format("{0}", Target);
        }
    }
}
