using MechWars.MapElements.Statistics;
using MechWars.Utils;

namespace MechWars.MapElements.Orders
{
    public class IdleOrder : ComplexOrder
    {
        public override string Name { get { return "Idle"; } }

        public MapElement AutoAttackTarget { get; private set; }

        IdleRotationOrder idleRotationOrder;
        AttackOrder attackOrder;

        public IdleOrder(MapElement mapElement)
            : base(mapElement)
        {
        }

        protected override void OnStart()
        {
            idleRotationOrder = new IdleRotationOrder(MapElement);
            GiveSubOrder(idleRotationOrder);
        }

        protected override void OnSubOrderUpdating()
        {
            if (SubOrder == idleRotationOrder)
            {
                if (MapElement.CanAttack)
                {
                    // TODO: PickClosestEnemyInRange causes major FPS problem
                    AutoAttackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
                    if (AutoAttackTarget != null && !AutoAttackTarget.Dying)
                        idleRotationOrder.Stop();
                }
            }
            else if (SubOrder == attackOrder)
            {
                if (AutoAttackTarget.Dying || !MapElement.HasMapElementInRange(AutoAttackTarget, StatNames.AttackRange))
                    attackOrder.Stop();
            }
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == idleRotationOrder) idleRotationOrder = null;
            else if (SubOrder == attackOrder) attackOrder = null;

            if (State != OrderState.Stopping)
            {
                if (AutoAttackTarget != null && !AutoAttackTarget.Dying &&
                    MapElement.HasMapElementInRange(AutoAttackTarget, StatNames.AttackRange))
                {
                    attackOrder = new AttackOrder(MapElement, AutoAttackTarget);
                    GiveSubOrder(attackOrder);
                }
                else
                {
                    idleRotationOrder = new IdleRotationOrder(MapElement);
                    GiveSubOrder(idleRotationOrder);
                }
            }
        }

        protected override void OnSubOrderFinished()
        {
            attackOrder = null;
            idleRotationOrder = new IdleRotationOrder(MapElement);
            GiveSubOrder(idleRotationOrder);
        }

        protected override string SpecificsToStringCore()
        {
            if (AutoAttackTarget == null) return base.SpecificsToStringCore();
            return string.Format("CurrentTarget: {0}", AutoAttackTarget);
        }
    }
}
