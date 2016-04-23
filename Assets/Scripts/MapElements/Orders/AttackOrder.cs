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

        public bool InRange
        {
            get
            {
                var dr = Target.Coords - MapElement.Coords;
                if (Mathf.Abs(dr.x) <= 1 && Mathf.Abs(dr.y) <= 1) return true;
                var range = MapElement.Stats[StatNames.Range];
                if (range == null) return false;
                var dist = dr.magnitude;
                return dist <= range.Value;
            }
        }

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
            if (!Target.Alive) return true;
            if (InRange)
            {
                MakeAttack();
                return false;
            }
            else throw new System.Exception(string.Format("Order {0} called, when not in range.", Name));
        }

        protected override bool StoppingUpdate()
        {
            if (!MapElement.canAttack)
                throw new System.Exception(string.Format(
                    "Order {0} called for MapElement {1}, but it cannot attack.", Name, MapElement));
            if (!Target.Alive) return true;
            if (!AttackingInProgress) return true;
            if (InRange)
            {
                MakeAttack();
                return false;
            }
            else throw new System.Exception(string.Format("Order {0} called, when not in range.", Name));
        }

        float cooldown = 0;

        void MakeAttack()
        {
            cooldown -= Time.deltaTime;

            if (cooldown <= 0)
            {
                if (attack == null)
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
                var direction = (Target.Coords - MapElement.Coords).normalized;
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
