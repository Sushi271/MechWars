using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders_OLD
{
    public class FollowAttackOrder_OLD : Order_OLD
    {
        MoveOrder_OLD move;
        AttackOrder_OLD attack;

        public Unit Unit { get; private set; }

        public MapElement CurrentTarget { get; private set; }
        public HashSet<MapElement> Targets { get; private set; }

        public FollowAttackOrder_OLD(Unit orderedUnit, IEnumerable<MapElement> target)
            : base("FollowAttack", orderedUnit)
        {
            AttackOrderHelper.AssertTargetsCanBeAttacked(target);

            Unit = (Unit)MapElement;
            Targets = new HashSet<MapElement>(target);
        }

        protected override bool RegularUpdate()
        {
            if (move != null && move.SingleMoveInProgress)
            {
                move.Update();
                return false;
            }

            if (attack != null && attack.AttackingInProgress)
            {
                attack.Update();
                return false;
            }

            if (CurrentTarget != null && !CurrentTarget.Alive)
            {
                Targets.Remove(CurrentTarget);
                CurrentTarget = null;
                move.OnSingleMoveFinished -= OnSingleMoveFinished;
                move = null;
                attack = null;
            }

            Targets.RemoveWhere(t => !t.Alive);

            if (CurrentTarget == null)
            {
                CurrentTarget = MapElement.PickClosestMapElementFrom(Targets);
                if (CurrentTarget == null) return true;
                else
                {
                    move = new MoveOrder_OLD(Unit, CurrentTarget.GetClosestFieldTo(Unit.Coords));
                    move.OnSingleMoveFinished += OnSingleMoveFinished;
                    attack = new AttackOrder_OLD(Unit, CurrentTarget);
                }
            }

            if (!MapElement.HasMapElementInRange(CurrentTarget, StatNames.AttackRange))
                move.Update();
            else attack.Update();
            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (move != null && move.SingleMoveInProgress)
            {
                move.Update();
                return move.Stopped;
            }

            if (attack != null && attack.AttackingInProgress)
            {
                attack.Update();
                return attack.Stopped;
            }

            return true;
        }

        protected override void TerminateCore()
        {
        }

        void OnSingleMoveFinished()
        {
            if (CurrentTarget.Alive)
                move.Destination = CurrentTarget.GetClosestFieldTo(Unit.Coords);
        }

        protected override void OnStopCalled()
        {
            move.Stop();
            attack.Stop();
        }

        protected override string SpecificsToString()
        {
            return string.Format("CurrentTarget: {0}, Targets: {1}", CurrentTarget, Targets.ToDebugMessage());
        }
    }
}
