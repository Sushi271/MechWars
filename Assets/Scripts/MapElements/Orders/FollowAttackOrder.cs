using MechWars.Utils;

namespace MechWars.MapElements.Orders
{
    public class FollowAttackOrder : Order
    {
        MoveOrder move;
        AttackOrder attack;

        public MapElement Target { get; private set; }
        
        public FollowAttackOrder(Unit orderedUnit, MapElement target)
            : base("FollowAttack", orderedUnit)
        {
            Target = target;
            
            move = new MoveOrder(Unit, target.Coords.Round());
            move.OnSingleMoveFinished += OnSingleMoveFinished;
            attack = new AttackOrder(orderedUnit, target);
        }

        protected override bool RegularUpdate()
        {
            if (move.SingleMoveInProgress)
                move.Update();
            else if (!Target.Alive) return true;
            else if (!attack.InRange)
                move.Update();
            else
            {
                attack.Update();
                if (attack.Stopped)
                    return true;
            }
            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (move.SingleMoveInProgress)
            {
                move.Update();
                if (move.Stopped) return true;
            }
            else if (!Target.Alive || !attack.InRange) return true;
            else
            {
                attack.Update();
                if (attack.Stopped)
                    return true;
            }
            return false;
        }

        void OnSingleMoveFinished()
        {
            if (Target.Alive)
                move.Destination = Target.Coords.Round();
        }

        protected override void OnStopCalled()
        {
            move.Stop();
            attack.Stop();
        }

        public override string ToString()
        {
            return string.Format("FollowAttack [ {0} ]", Target);
        }
    }
}
