using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class EscortOrder : ComplexOrder
    {
        public override string Name { get { return "Escort"; } }

        public Unit Unit { get; private set; }
        public HashSet<Unit> Targets { get; private set; }

        public MapElement AttackTarget { get; private set; }
        IVector2 AttackTargetClosestCoords { get { return AttackTarget.GetClosestFieldTo(Unit.Coords); } }

        IVector2 Destination { get; set; }
        bool DestinationInRange { get { return Unit.HasPositionInRange(Destination, StatNames.AttackRange); } }

        MoveOrder moveOrder;
        AttackOrder attackOrder;

        float nextAutoAttackScanIn;
        bool lostTrack;

        public EscortOrder(Unit unit, IEnumerable<Unit> targets)
            : base(unit)
        {
            Unit = unit;
            Targets = new HashSet<Unit>(targets);
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertMapElementHasAnyAttacks(MapElement));
            if (Failed) return;

            UpdateEscortData();
            if (Succeeded) return;

            GiveNewSubOrder();
        }

        protected override void OnUpdate()
        {
            UpdateEscortData();
            if (!AttackTarget.IsTrueNull())
                CorrectTarget();
        }

        void CorrectTarget()
        {
            if (!AttackTarget.IsGhost)
            {
                if (AttackTarget.Dying) return;

                var targetVisible = AttackTarget.VisibleToArmies[MapElement.Army];
                if (targetVisible) return;

                if (AttackTarget.CanHaveGhosts)
                {
                    var ghost = AttackTarget.Ghosts[MapElement.Army];
                    if (ghost == null)
                        throw new System.Exception("AttackTarget has no Ghost, though it CanHaveGhosts and it's not visible by attacker Army.");
                    AttackTarget = ghost;
                }
                else lostTrack = true;
            }
            else
            {
                if (!AttackTarget.GhostRemoved) return;
                AttackTarget = AttackTarget.OriginalMapElement;
            }
        }

        protected override void OnSubOrderUpdating()
        {
            if (SubOrder.State == OrderState.Stopping) return;

            if (SubOrder == moveOrder)
            {
                if (nextAutoAttackScanIn > 0)
                {
                    nextAutoAttackScanIn -= Time.deltaTime;
                    if (nextAutoAttackScanIn < 0)
                        nextAutoAttackScanIn = 0;
                }
                if (DestinationInRange && nextAutoAttackScanIn == 0)
                {
                    var closest = MapElement.PickClosestEnemyInRange(StatNames.AttackRange, true);
                    if (closest != null && MapElement.HasMapElementInRange(closest, StatNames.AttackRange))
                    {
                        AttackTarget = closest;
                        moveOrder.Stop();
                    }
                    else
                    {
                        var interval = Globals.Instance.autoAttackScanInterval;
                        nextAutoAttackScanIn = Random.Range(0.9f * interval, 1.1f * interval);
                    }
                }
            }
            else if (SubOrder == attackOrder)
            {
                if (lostTrack || !DestinationInRange)
                    attackOrder.Stop();
            }
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == moveOrder) moveOrder = null;
            else if (SubOrder == attackOrder)
            {
                lostTrack = false;
                attackOrder = null;
                AttackTarget = null;
            }

            if (State != OrderState.Stopping && !Conclusive)
                GiveNewSubOrder();
        }

        protected override void OnSubOrderFinished()
        {
            attackOrder = null;
            AttackTarget = null;
            GiveNewSubOrder();
        }
        
        void GiveNewSubOrder()
        {
            if (DestinationInRange)
            {
                AttackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange, true);
                if (AttackTarget != null)
                {
                    attackOrder = new AttackOrder(MapElement, AttackTarget);
                    GiveSubOrder(attackOrder);
                }
            }
            else
            {
                moveOrder = new MoveOrder(Unit, Destination, true);
                GiveSubOrder(moveOrder);
            }
        }

        void UpdateEscortData()
        {
            Targets.RemoveWhere(t => t.Dying);
            if (Targets.Empty())
            {
                if (moveOrder != null) moveOrder.Stop();
                if (attackOrder != null) attackOrder.Stop();
                Succeed();
            }
            else
            {
                Destination = Targets.Average2(t => t.Coords).Round();
                if (moveOrder != null)
                    moveOrder.Destination = Destination;
            }
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("Targets: {0}, AttackTarget: {1}", Targets.ToDebugMessage(),
                AttackTarget != null ? AttackTarget.ToString() : "NONE");
        }
    }
}
