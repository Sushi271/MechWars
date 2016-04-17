using MechWars.MapElements;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.Orders
{
    public class FollowAttackOrder : Order
    {
        MoveOrder move;
        AttackOrder attack;

        public MapElement Target { get; private set; }
        
        public FollowAttackOrder(Unit orderedUnit, MapElement target)
            : base("Attack", orderedUnit)
        {
            Target = target;
            
            move = new MoveOrder(Unit, target.Coords.Round());
            move.OnSingleMoveFinished += OnSingleMoveFinished;
            attack = new AttackOrder(orderedUnit, target);
        }

        protected override bool RegularUpdate()
        {
            if (move.SingleMoveInProgress || !attack.InRange)
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
            else if (!attack.InRange) return true;
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
