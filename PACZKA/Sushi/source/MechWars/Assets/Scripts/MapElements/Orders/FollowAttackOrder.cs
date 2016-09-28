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
            CorrectTargets();
        }

        void CorrectTargets()
        {
            var toRemove = new HashSet<MapElement>();
            var toAdd = new HashSet<MapElement>();
            foreach (var t in Targets)
            {
                if (!t.IsGhost)
                {
                    var targetVisible = t.VisibleToArmies[MapElement.Army];
                    if (targetVisible) continue;

                    if (t.CanHaveGhosts)
                    {
                        var ghost = t.Ghosts[MapElement.Army];
                        if (ghost == null)
                            throw new System.Exception("Target has no Ghost, though it CanHaveGhosts and it's not visible by attacker Army.");
                        toAdd.Add(ghost);
                        if (t == CurrentTarget)
                            CurrentTarget = ghost;
                    }
                    toRemove.Add(t);
                }
                else
                {
                    if (!t.GhostRemoved) continue;
                    if (t.OriginalMapElement != null)
                        toAdd.Add(t.OriginalMapElement);
                    toRemove.Add(t);
                    if (t == CurrentTarget)
                        CurrentTarget = t.OriginalMapElement;
                }
            }
            Targets.ExceptWith(toRemove);
            Targets.UnionWith(toAdd);
            if (CurrentTarget == null)
                UpdateCurrentTarget();
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
                if (CurrentTarget != null && !CurrentTarget.Dying &&
                    !MapElement.HasMapElementInRange(CurrentTarget, StatNames.AttackRange))
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
