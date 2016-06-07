using System;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using MechWars.MapElements.Statistics;

namespace MechWars.MapElements.Orders_OLD
{
    public class EscortOrder_OLD : Order_OLD
    {
        MoveOrder_OLD move;
        AttackOrder_OLD attack;
        MapElement attackTarget;
        bool targetLost;

        public Unit Unit { get; private set; }
        public HashSet<Unit> Targets { get; set; }
        public IVector2 Destination { get; private set; }

        public EscortOrder_OLD(Unit orderedUnit, IEnumerable<Unit> target)
            : base("Escort", orderedUnit)
        {
            Unit = (Unit)MapElement;
            Targets = new HashSet<Unit>(target);

            Destination = PickDestination();
            move = new MoveOrder_OLD(Unit, Destination, true);
        }

        protected override bool RegularUpdate()
        {
            if (move.SingleMoveInProgress)
            {
                move.Update();
                return false;
            }

            if (attack != null && attack.AttackingInProgress)
            {
                attack.Update();
                return false;
            }

            Targets.RemoveWhere(t => !t.Alive);
            if (Targets.Empty())
                return true;

            Destination = PickDestination();

            if (attack == null)
            {
                attackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
                if (attackTarget != null)
                    attack = new AttackOrder_OLD(Unit, attackTarget);
            }
            else if (targetLost || !attackTarget.Alive
                || !MapElement.HasMapElementInRange(attackTarget, StatNames.AttackRange))
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

            if (attack != null && MapElement.HasPositionInRange(Destination, StatNames.AttackRange))
            {
                attack.Update();
                return false;
            }

            move.Destination = PickDestination();
            move.Update();
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

        IVector2 PickDestination()
        {
            if (Targets.Empty())
                throw new System.Exception("Cannot pick destination - no more targets to escort.");
            return Targets.Average(t => t.Coords).Round();
        }

        protected override string SpecificsToString()
        {
            return string.Format("Targets: {0}", Targets.ToDebugMessage());
        }
    }
}
