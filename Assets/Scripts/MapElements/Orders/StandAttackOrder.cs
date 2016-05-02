using MechWars.Utils;

namespace MechWars.MapElements.Orders
{
    public class StandAttackOrder : Order<MapElement>
    {
        AttackOrder attack;
        
        public MapElement Target { get; private set; }

        public StandAttackOrder(Unit orderedUnit, MapElement target)
            : base("Attack", orderedUnit)
        {
            Target = target;
            attack = new AttackOrder(orderedUnit, target);
        }

        public StandAttackOrder(Building orderedBuilding, MapElement target)
            : base("Attack", orderedBuilding)
        {
            Target = target;
            attack = new AttackOrder(orderedBuilding, target);
        }        

        protected override bool RegularUpdate()
        {
            if (attack.AttackingInProgress)
            {
                attack.Update();
                return attack.Stopped;
            }
            
            if (!Target.Alive) return true;

            if (MapElement.MapElementInRange(Target))
                attack.Update();

            return false;
        }

        protected override bool StoppingUpdate()
        {
            if (attack.AttackingInProgress)
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
            attack.Stop();
        }

        public override string ToString()
        {
            return string.Format("StandAttack [ {0} ]", Target);
        }
    }
}
