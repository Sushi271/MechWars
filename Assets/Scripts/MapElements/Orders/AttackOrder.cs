using MechWars.MapElements.Attacks;
using MechWars.MapElements.Jobs;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class AttackOrder : Order<MapElement>
    {
        Attack attack;

        public MapElement Target { get; private set; }
        public bool AttackingInProgress { get; private set; }

        public AttackOrder(Unit orderedUnit, MapElement target)
            : base("Attack", orderedUnit)
        {
            Target = target;
        }

        public AttackOrder(Building orderedBuilding, MapElement target)
            : base("Attack", orderedBuilding)
        {
            Target = target;
        }

        protected override bool RegularUpdate()
        {
            if (!MapElement.canAttack)
                throw new System.Exception(string.Format(
                    "Order {0} called for MapElement {1}, but it cannot attack.", Name, MapElement));
            if (!AttackingInProgress && !Target.Alive) return true;
            TryMakeAttack();
            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (!MapElement.canAttack)
                throw new System.Exception(string.Format(
                    "Order {0} called for MapElement {1}, but it cannot attack.", Name, MapElement));
            if (!AttackingInProgress) return true;
            Debug.Log("" + MapElement);
            TryMakeAttack();
            return false;
        }

        protected override void TerminateCore()
        {
        }

        float cooldown = 0;

        void TryMakeAttack()
        {
            Vector2 coords;
            MapElement.MapElementInRange(Target, out coords);
            if (coords != null)
                MakeAttack(coords);
            else throw new System.Exception(string.Format("Order {0} called, when not in range.", Name));
        }

        void MakeAttack(Vector2 coords)
        {
            if (!MapElement.JobQueue.Empty) return;

            if (attack != null)
            {
                attack.ExecuteStep();
                if (attack.Finished)
                {
                    attack = null;
                    AttackingInProgress = false;
                }
            }
            else if (Target.Alive && !Stopping && !Stopped)
            {
                cooldown -= Time.deltaTime;
                if (cooldown <= 0)
                {
                    var attackSpeedStat = MapElement.Stats[StatNames.AttackSpeed];
                    float attackSpeed = 1;
                    if (attackSpeedStat != null && attackSpeedStat.Value > 0)
                        attackSpeed = attackSpeedStat.Value;
                    float attackInterval = 1 / attackSpeed;

                    cooldown += attackInterval;
                    attack = PickAttack();
                    if (attack != null)
                    {
                        attack.Initialize(MapElement, Target);
                        var direction = coords - MapElement.Coords;
                        var angle = -UnityExtensions.AngleFromTo(Vector2.up, direction);
                        MapElement.JobQueue.Add(new RotateJob(MapElement, angle));
                        AttackingInProgress = true;
                    }
                }
            }
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

        public override string ToString()
        {
            return string.Format("Attack [ {0} ]", Target);
        }
    }
}
