using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders
{
    public class FollowAttackOrder : ComplexOrder
    {
        public override string Name { get { return "FollowAttack"; } }

        public Unit Unit { get; private set; }
        public HashSet<MapElement> Targets { get; private set; }

        public MapElement CurrentTarget { get; private set; }
        IVector2 CurrentTargetClosestCoords { get { return CurrentTarget.GetClosestFieldTo(Unit.Coords); } }

        MoveOrder moveOrder;
        AttackOrder attackOrder;

        public FollowAttackOrder(Unit unit, IEnumerable<MapElement> targets)
            : base(unit)
        {
            Unit = unit;
            Targets = new HashSet<MapElement>(targets);
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertMapElementsCanBeAttacked(Targets));
            TryFail(OrderResultAsserts.AssertMapElementHasAnyAttacks(MapElement));
            if (Failed) return;

            UpdateCurrentTarget();
        }

        protected override void OnUpdate()
        {
            Targets.RemoveWhere(t => t.Dying);
        }

        protected override void OnSubOrderUpdating()
        {
            if (SubOrder == moveOrder)
            {
                if (CurrentTarget.Dying)
                    moveOrder.Stop();
                else
                {
                    moveOrder.Destination = CurrentTargetClosestCoords;
                    if (MapElement.HasMapElementInRange(CurrentTarget, StatNames.AttackRange))
                        moveOrder.Stop();
                }
            }
            else if (SubOrder == attackOrder)
            {
                if (!CurrentTarget.Dying && !MapElement.HasMapElementInRange(CurrentTarget, StatNames.AttackRange))
                    attackOrder.Stop();
            }
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == moveOrder) moveOrder = null;
            else if (SubOrder == attackOrder) attackOrder = null;

            if (State != OrderState.Stopping)
            {
                if (!CurrentTarget.Dying)
                {
                    if (MapElement.HasMapElementInRange(CurrentTarget, StatNames.AttackRange))
                    {
                        attackOrder = new AttackOrder(Unit, CurrentTarget);
                        GiveSubOrder(attackOrder);
                    }
                    else
                    {
                        moveOrder = new MoveOrder(Unit, CurrentTargetClosestCoords, true);
                        GiveSubOrder(moveOrder);
                    }
                }
                else UpdateCurrentTarget();
            }
        }

        protected override void OnSubOrderFinished()
        {
            attackOrder = null;
            UpdateCurrentTarget();
        }

        void UpdateCurrentTarget()
        {
            Targets.RemoveWhere(t => t.Dying);
            CurrentTarget = MapElement.PickClosestMapElementFrom(Targets);
            if (CurrentTarget == null) Succeed();
            else
            {
                moveOrder = new MoveOrder(Unit, CurrentTargetClosestCoords, true);
                GiveSubOrder(moveOrder);
            }
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("CurrentTarget: {0}, Targets: {1}", CurrentTarget, Targets.ToDebugMessage());
        }
    }
}
