using MechWars.Utils;

namespace MechWars.MapElements.Orders
{
    public class AttackMoveOrder : Order<Unit>
    {
        MoveOrder move;
        FollowAttackOrder attack;
        MapElement attackTarget;

        public Unit Unit { get; private set; }
        public IVector2 Destination { get; set; }

        public AttackMoveOrder(Unit orderedUnit, IVector2 destination)
            : base("AttackMove", orderedUnit)
        {
            Unit = (Unit)MapElement;
            Destination = destination;

            move = new MoveOrder(Unit, destination);
        }

        protected override bool RegularUpdate()
        {
            if (move.SingleMoveInProgress)
            {
                move.Update();
                return move.Stopped;
            }

            if (attack == null)
            {
                attackTarget = MapElement.AcquireTarget();
                if (attackTarget != null)
                    attack = new FollowAttackOrder(Unit, attackTarget);
            }
            if (attack != null)
            {
                if (!attack.Stopped)
                    attack.Update();
                else
                {
                    attack = null;
                    attackTarget = null;
                }
                return false;
            }

            move.Update();
            return move.Stopped;
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

        public override string ToString()
        {
            return string.Format("AttackMove [ {0} ]", Destination);
        }
    }
}
