using MechWars.MapElements.Statistics;
using MechWars.Utils;

namespace MechWars.MapElements.Orders_OLD
{
    public class AttackMoveOrder_OLD : Order_OLD
    {
        MoveOrder_OLD move;
        FollowAttackOrder_OLD attack;
        MapElement attackTarget;

        public Unit Unit { get; private set; }
        public IVector2 Destination { get; set; }

        public AttackMoveOrder_OLD(Unit orderedUnit, IVector2 destination)
            : base("AttackMove", orderedUnit)
        {
            Unit = (Unit)MapElement;
            Destination = destination;

            move = new MoveOrder_OLD(Unit, destination);
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
                attackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
                if (attackTarget != null)
                    attack = new FollowAttackOrder_OLD(Unit, attackTarget.AsEnumerable());
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


        protected override string SpecificsToString()
        {
            return string.Format("{0}", Destination);
        }
    }
}
