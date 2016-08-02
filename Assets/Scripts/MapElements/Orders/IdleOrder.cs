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

        float nextScanIn;

        bool lostTrack;

        public IdleOrder(MapElement mapElement)
            : base(mapElement)
        {
            AutoAttackTarget = MapElement.Null;
        }

        protected override void OnStart()
        {
            idleRotationOrder = new IdleRotationOrder(MapElement);
            GiveSubOrder(idleRotationOrder);

            nextScanIn = Random.Range(0, Globals.Instance.autoAttackScanInterval);
        }

        protected override void OnUpdate()
        {
            if (AutoAttackTarget.IsNotNull)
                CorrectTarget();
        }

        void CorrectTarget()
        {
            if (!AutoAttackTarget.IsGhost)
            {
                if (AutoAttackTarget.Dying)
                    return;

                var targetVisible = AutoAttackTarget.VisibleToArmies[MapElement.Army];
                if (targetVisible) return;

                if (AutoAttackTarget.CanHaveGhosts)
                {
                    var ghost = AutoAttackTarget.Ghosts[MapElement.Army];
                    if (ghost.IsNull)
                        throw new System.Exception("AutoAttackTarget has no Ghost, though it CanHaveGhosts and it's not visible by attacker Army.");
                    AutoAttackTarget = ghost;
                }
                else lostTrack = true;
            }
            else
            {
                if (!AutoAttackTarget.GhostRemoved) return;
                AutoAttackTarget = AutoAttackTarget.OriginalMapElement;
            }
        }

        protected override void OnSubOrderUpdating()
        {
            if (SubOrder == idleRotationOrder)
            {
                if (nextScanIn > 0)
                {
                    nextScanIn -= Time.deltaTime;
                    if (nextScanIn < 0)
                        nextScanIn = 0;
                }
                if (MapElement.CanAttack && nextScanIn == 0)
                {
                    var closest = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
                    if (closest.IsNotNull && MapElement.HasMapElementInRange(closest, StatNames.AttackRange))
                        AutoAttackTarget = closest;
                    if (AutoAttackTarget.IsNotNull && !AutoAttackTarget.Dying)
                        idleRotationOrder.Stop();
                    else
                    {
                        AutoAttackTarget = MapElement.Null;

                        var interval = Globals.Instance.autoAttackScanInterval;
                        nextScanIn = Random.Range(0.9f * interval, 1.1f * interval);
                    }
                }
            }
            else if (SubOrder == attackOrder)
            {
                if (lostTrack || AutoAttackTarget.IsNull || AutoAttackTarget.Dying ||
                    !MapElement.HasMapElementInRange(AutoAttackTarget, StatNames.AttackRange))
                    attackOrder.Stop();
            }
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == idleRotationOrder) idleRotationOrder = null;
            else if (SubOrder == attackOrder)
            {
                lostTrack = false;
                attackOrder = null;
                AutoAttackTarget = MapElement.Null;
            }

            if (State != OrderState.Stopping)
            {
                if (AutoAttackTarget.IsNotNull && !AutoAttackTarget.Dying &&
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
            if (AutoAttackTarget.IsNull) return base.SpecificsToStringCore();
            return string.Format("CurrentTarget: {0}", AutoAttackTarget);
        }
    }
}
