using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class StandAttackOrder : ComplexOrder
    {
        // TODO: Why there is RotateOrder inside this? Attack rotates anyway!
        // - Because if target is out of range, it will be "tracked"

        public override string Name { get { return "StandAttack"; } }
        
        public HashSet<MapElement> Targets { get; private set; }

        public MapElement CurrentTarget { get; private set; }
        float CurrentTargetAngle
        {
            get
            {
                var targetCoords = CurrentTarget.GetClosestAimTo(MapElement.Coords).AsHorizontalVector2();
                var direction = targetCoords - MapElement.Coords;
                var angle = UnityExtensions.AngleFromToXZ(Vector2.up, direction);
                return angle;
            }
        }

        RotateOrder rotateOrder;
        AttackOrder attackOrder;

        public StandAttackOrder(MapElement mapElement, IEnumerable<MapElement> targets)
            : base(mapElement)
        {
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
            if (CurrentTarget.IsTrueNull())
                UpdateCurrentTarget();
        }

        protected override void OnSubOrderUpdating()
        {
            if (SubOrder == rotateOrder)
            {
                var closest = MapElement.PickClosestMapElementFrom(Targets);
                var currentInRange = MapElement.HasMapElementInRange(CurrentTarget, StatNames.AttackRange);
                if (closest != CurrentTarget || CurrentTarget.Dying || currentInRange)
                    rotateOrder.Stop();
                else rotateOrder.TargetRotation = CurrentTargetAngle;
            }
            else if (SubOrder == attackOrder)
            {
                if (!CurrentTarget.Dying && !MapElement.HasMapElementInRange(CurrentTarget, StatNames.AttackRange))
                    attackOrder.Stop();
            }
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == rotateOrder) rotateOrder = null;
            else if (SubOrder == attackOrder) attackOrder = null;

            if (State != OrderState.Stopping)
                if (!CurrentTarget.Dying)
                    GiveNewSubOrder();
                else UpdateCurrentTarget();
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
            if (CurrentTarget != null) GiveNewSubOrder();
            else Succeed();
        }

        void GiveNewSubOrder()
        {
            if (MapElement.HasMapElementInRange(CurrentTarget, StatNames.AttackRange))
            {
                attackOrder = new AttackOrder(MapElement, CurrentTarget);
                GiveSubOrder(attackOrder);
            }
            else
            {
                rotateOrder = new RotateOrder(MapElement, true, CurrentTargetAngle);
                GiveSubOrder(rotateOrder);
            }
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("CurrentTarget: {0}, Targets: {1}", CurrentTarget, Targets.ToDebugMessage());
        }
    }
}
