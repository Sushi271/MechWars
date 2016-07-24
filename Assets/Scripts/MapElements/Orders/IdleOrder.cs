using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class IdleOrder : ComplexOrder
    {
        public override string Name { get { return "Idle"; } }

        public MapElement AutoAttackTarget { get; private set; }

        IdleRotationOrder idleRotationOrder;
        AttackOrder attackOrder;

        float nextAggroScanIn;

        public IdleOrder(MapElement mapElement)
            : base(mapElement)
        {
        }

        protected override void OnStart()
        {
            idleRotationOrder = new IdleRotationOrder(MapElement);
            GiveSubOrder(idleRotationOrder);
            
            nextAggroScanIn = Random.Range(0, Globals.Instance.aggroScanInterval);
        }

        protected override void OnSubOrderUpdating()
        {
            if (SubOrder == idleRotationOrder)
            {
                if (nextAggroScanIn > 0)
                {
                    nextAggroScanIn -= Time.deltaTime;
                    if (nextAggroScanIn < 0)
                        nextAggroScanIn = 0;
                }
                if (MapElement.CanAttack && nextAggroScanIn == 0)
                {
                    var closest = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
                    if (closest != null && MapElement.HasMapElementInRange(closest, StatNames.AttackRange))
                        AutoAttackTarget = closest;
                    if (AutoAttackTarget != null && !AutoAttackTarget.Dying)
                        idleRotationOrder.Stop();
                    else
                    {
                        var interval = Globals.Instance.aggroScanInterval;
                        nextAggroScanIn = Random.Range(0.9f * interval, 1.1f * interval);
                    }
                }
            }
            else if (SubOrder == attackOrder)
            {
                if (AutoAttackTarget.Dying || !MapElement.HasMapElementInRange(AutoAttackTarget, StatNames.AttackRange))
                    attackOrder.Stop();
            }
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == idleRotationOrder) idleRotationOrder = null;
            else if (SubOrder == attackOrder) attackOrder = null;

            if (State != OrderState.Stopping)
            {
                if (AutoAttackTarget != null && !AutoAttackTarget.Dying &&
                    MapElement.HasMapElementInRange(AutoAttackTarget, StatNames.AttackRange))
                {
                    attackOrder = new AttackOrder(MapElement, AutoAttackTarget);
                    GiveSubOrder(attackOrder);
                }
                else
                {
                    idleRotationOrder = new IdleRotationOrder(MapElement);
                    GiveSubOrder(idleRotationOrder);
                }
            }
        }

        protected override void OnSubOrderFinished()
        {
            attackOrder = null;
            idleRotationOrder = new IdleRotationOrder(MapElement);
            GiveSubOrder(idleRotationOrder);
        }

        protected override string SpecificsToStringCore()
        {
            if (AutoAttackTarget == null) return base.SpecificsToStringCore();
            return string.Format("CurrentTarget: {0}", AutoAttackTarget);
        }
    }
}
