using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class AttackMoveOrder : ComplexOrder
    {
        public override string Name { get { return "AttackMove"; } }

        public Unit Unit { get; private set; }

        public MapElement AttackTarget { get; private set; }

        IVector2 Destination { get; set; }

        MoveOrder moveOrder;
        FollowAttackOrder followAttackOrder;

        float nextAutoAttackScanIn;

        bool lostTrack;

        public AttackMoveOrder(Unit unit, IVector2 destination)
            : base(unit)
        {
            Unit = unit;
            Destination = destination;
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertMapElementHasAnyAttacks(MapElement));
            if (Failed) return;

            GiveNewSubOrder();
        }

        protected override void OnUpdate()
        {
            if (AttackTarget != null)
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
                if (nextAutoAttackScanIn == 0)
                {
                    var closest = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
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
            else if (SubOrder == followAttackOrder)
            {
                if (lostTrack)
                    followAttackOrder.Stop();
            }
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == moveOrder) moveOrder = null;
            else if (SubOrder == followAttackOrder)
            {
                lostTrack = false;
                followAttackOrder = null;
                AttackTarget = null;
            }

            if (State != OrderState.Stopping && !Conclusive)
                GiveNewSubOrder();
        }

        protected override void OnSubOrderFinished()
        {
            if (SubOrder == moveOrder)
            {
                moveOrder = null;
                Succeed();
            }
            else if (SubOrder == followAttackOrder)
            {
                followAttackOrder = null;
                AttackTarget = null;
                GiveNewSubOrder();
            }
        }
        
        void GiveNewSubOrder()
        {
            AttackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
            if (AttackTarget != null)
            {
                followAttackOrder = new FollowAttackOrder(Unit, AttackTarget.AsEnumerable());
                GiveSubOrder(followAttackOrder);
            }
            else
            {
                moveOrder = new MoveOrder(Unit, Destination);
                GiveSubOrder(moveOrder);
            }
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("Destination: {0}, AttackTarget: {1}", Destination,
                AttackTarget != null ? AttackTarget.ToString() : "NONE");
        }
    }
}
