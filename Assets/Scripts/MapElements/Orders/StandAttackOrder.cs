using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders
{
    public class StandAttackOrder : Order<MapElement>
    {
        AttackOrder attack;
        
        public MapElement CurrentTarget { get; private set; }
        public HashSet<MapElement> Targets { get; private set; }

        public StandAttackOrder(Unit orderedUnit, IEnumerable<MapElement> targets)
            : base("Attack", orderedUnit)
        {
            AttackOrderHelper.AssertTargetsCanBeAttacked(targets);

            Targets = new HashSet<MapElement>(targets);
        }

        public StandAttackOrder(Building orderedBuilding, IEnumerable<MapElement> targets)
            : base("Attack", orderedBuilding)
        {
            AttackOrderHelper.AssertTargetsCanBeAttacked(targets);

            Targets = new HashSet<MapElement>(targets);
            attack = new AttackOrder(orderedBuilding, CurrentTarget);
        }        

        protected override bool RegularUpdate()
        {
            if (attack != null && attack.AttackingInProgress)
            {
                attack.Update();
                return false;
            }

            if (CurrentTarget != null && !CurrentTarget.Alive)
            {
                Targets.Remove(CurrentTarget);
                CurrentTarget = null;
                attack = null;
            }

            Targets.RemoveWhere(t => !t.Alive);

            if (CurrentTarget == null || !MapElement.MapElementInRange(CurrentTarget))
            {
                CurrentTarget = AttackOrderHelper.PickTarget(MapElement, Targets);
                if (CurrentTarget == null) return true;
                else attack = AttackOrder.Create(MapElement, CurrentTarget);
            }

            if (MapElement.MapElementInRange(CurrentTarget))
                attack.Update();

            return false;
        }

        protected override bool StoppingUpdate()
        {
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
        
        protected override void OnStopCalled()
        {
            if (attack != null)
                attack.Stop();
        }

        protected override string SpecificsToString()
        {
            return string.Format("CurrentTarget: {0}, Targets: {1}", CurrentTarget, Targets.ToDebugMessage());
        }
    }
}
