using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders
{
    public class FollowAttackOrder : Order<Unit>
    {
        MoveOrder move;
        AttackOrder attack;

        public Unit Unit { get; private set; }

        public MapElement CurrentTarget { get; private set; }
        public HashSet<MapElement> Targets { get; private set; }

        public FollowAttackOrder(Unit orderedUnit, IEnumerable<MapElement> target)
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
                CurrentTarget = AttackOrderHelper.PickTarget(MapElement, Targets);
                if (CurrentTarget == null) return true;
                else
                {
                    move = new MoveOrder(Unit, CurrentTarget.GetClosestFieldTo(Unit.Coords).Round());
                    move.OnSingleMoveFinished += OnSingleMoveFinished;
                    attack = new AttackOrder(Unit, CurrentTarget);
                }
            }

            if (!MapElement.MapElementInRange(CurrentTarget))
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
                move.Destination = CurrentTarget.GetClosestFieldTo(Unit.Coords).Round();
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
