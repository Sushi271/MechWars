using MechWars.MapElements;
using MechWars.MapElements.Attacks;
using UnityEngine;

namespace MechWars.Orders
{
    public class AttackOrder : Order
    {
        Attack attack;

        public MapElement Target { get; private set; }
        public bool AttackingInProgress { get; private set; }

        public bool InRange
        {
            get
            {
                var dr = Target.Coords - Unit.Coords;
                if (Mathf.Abs(dr.x) <= 1 && Mathf.Abs(dr.y) <= 1) return true;
                var range = Unit.Stats[StatNames.Range];
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

        protected override bool RegularUpdate()
        {
            if (!Target.Alive) return true;
            if (InRange) MakeAttack();
            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (!Target.Alive) return true;
            if (!AttackingInProgress) return true;
            if (InRange) MakeAttack();
            return false;
        }

        float cooldown = 0;

        void MakeAttack()
        {
            cooldown -= Time.deltaTime;

            if (cooldown <= 0)
            {
                if (attack == null)
                {
                    var attackSpeedStat = Unit.Stats[StatNames.AttackSpeed];
                    float attackSpeed = 1;
                    if (attackSpeedStat != null && attackSpeedStat.Value > 0)
                        attackSpeed = attackSpeedStat.Value;
                    float attackInterval = 1 / attackSpeed;

                    cooldown += attackInterval;
                    attack = PickAttack();
                    if (attack != null)
                    {
                        attack.Initialize(Unit, Target);
                        AttackingInProgress = true;
                    }
                }
                else
                {
                    attack = null;
                    AttackingInProgress = false;
                    throw new System.Exception(string.Format("Attack.ExecuteStep is taking too long " +
                        "(time exceeds Unit's [ {0} ] 'Attack speed' attribute value).", Unit));
                }
            }
            if (attack != null)
            {
                var direction = (Target.Coords - Unit.Coords).normalized;
                var direction3 = new Vector3(direction.x, 0, direction.y);
                Unit.transform.localRotation = Quaternion.LookRotation(direction3);

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
            var attacks = Unit.GetComponents<Attack>();
            if (attacks.Length == 0)
            {
                Debug.LogWarning(string.Format("Unit {0} has no Attacks.", Unit));
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
