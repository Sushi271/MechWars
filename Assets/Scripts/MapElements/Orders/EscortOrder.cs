using System;
using MechWars.Utils;

namespace MechWars.MapElements.Orders
{
    public class EscortOrder : Order<Unit>
    {
        MoveOrder move;
        AttackOrder attack;
        MapElement attackTarget;
        bool targetLost;

        public Unit Unit { get; private set; }
        public Unit Target { get; set; }

        public EscortOrder(Unit orderedUnit, Unit target)
            : base("Escort", orderedUnit)
        {
            Unit = (Unit)MapElement;
            Target = target;

            move = new MoveOrder(Unit, target.Coords.Round(), true);
            move.OnSingleMoveFinished += OnSingleMoveFinished;
        }

        protected override bool RegularUpdate()
        {
            if (move.SingleMoveInProgress)
            {
                move.Update();
                return false;
            }

            if (attack == null)
            {
                attackTarget = MapElement.AcquireTarget();
                if (attackTarget != null)
                    attack = new AttackOrder(Unit, attackTarget);
            }
            else if (targetLost || !attackTarget.Alive
                || !MapElement.MapElementInRange(attackTarget))
            {
                targetLost = true;
                if (!attack.Stopping)
                    attack.Stop();
                if (attack.Stopped)
                {
                    attack = null;
                    attackTarget = null;
                }
            }

            if (attack != null && MapElement.MapElementInRange(Target))
                attack.Update();
            else
            {
                if (Target.Alive)
                {
                    move.Destination = Target.Coords.Round();
                    move.Update();
                }
                return !Target.Alive;
            }
            
            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (move.SingleMoveInProgress)
            {
                move.Update();
                return move.Stopped;
            }

            if (attack != null && !attack.Stopped)
            {
                attack.Update();
                return attack.Stopped;
            }

            return true;
        }

        protected override void TerminateCore()
        {
        }

        protected override void OnStopCalled()
        {
            move.Stop();
            if (attack != null)
                attack.Stop();
        }

        void OnSingleMoveFinished()
        {
            if (Target.Alive)
                move.Destination = Target.Coords.Round();
        }

        protected override string SpecificsToString()
        {
            return string.Format("{0}", Target);
        }
    }
}
