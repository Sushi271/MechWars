using MechWars.MapElements.Attacks;
using MechWars.MapElements.Statistics;
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
            cooldown -= Time.deltaTime;

            if (cooldown <= 0)
            {
                if (attack == null && Target.Alive && !Stopping && !Stopped)
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
                        AttackingInProgress = true;
                    }
                }
                else
                {
                    attack = null;
                    AttackingInProgress = false;
                    throw new System.Exception(string.Format("Attack.ExecuteStep is taking too long " +
                        "(time exceeds MapElement's [ {0} ] 'Attack speed' attribute value).", MapElement));
                }
            }
            if (attack != null)
            {
                var direction = (coords - MapElement.Coords).normalized;
                var direction3 = new Vector3(direction.x, 0, direction.y);
                MapElement.transform.localRotation = Quaternion.LookRotation(direction3);

                attack.ExecuteStep();
                if (attack.Finished)
                {
                    attack = null;
                    AttackingInProgress = false;
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
