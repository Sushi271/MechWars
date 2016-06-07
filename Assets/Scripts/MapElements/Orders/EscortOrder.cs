using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders
{
    public class EscortOrder : ComplexOrder
    {
        public override string Name { get { return "Escort"; } }

        public Unit Unit { get; private set; }
        public HashSet<Unit> Targets { get; private set; }

        public MapElement AttackTarget { get; private set; }
        IVector2 AttackTargetClosestCoords { get { return AttackTarget.GetClosestFieldTo(Unit.Coords); } }

        IVector2 Destination { get; set; }
        bool DestinationInRange { get { return Unit.HasPositionInRange(Destination, StatNames.AttackRange); } }

        MoveOrder moveOrder;
        AttackOrder attackOrder;

        public EscortOrder(Unit unit, IEnumerable<Unit> targets)
            : base(unit)
        {
            Unit = unit;
            Targets = new HashSet<Unit>(targets);
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertMapElementHasAnyAttacks(MapElement));
            if (Failed) return;

            UpdateEscortData();
            if (Succeeded) return;

            GiveNewSubOrder();
        }

        protected override void OnUpdate()
        {
            UpdateEscortData();
        }

        protected override void OnSubOrderUpdating()
        {
            if (SubOrder.State == OrderState.Stopping) return;

            if (SubOrder == moveOrder)
            {
                if (DestinationInRange)
                {
                    AttackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
                    if (AttackTarget != null)
                        moveOrder.Stop();
                }
            }
            else if (SubOrder == attackOrder)
            {
                if (!DestinationInRange)
                    attackOrder.Stop();
            }
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == moveOrder) moveOrder = null;
            else if (SubOrder == attackOrder)
            {
                attackOrder = null;
                AttackTarget = null;
            }

            if (State != OrderState.Stopping && !Conclusive)
                GiveNewSubOrder();
        }

        protected override void OnSubOrderFinished()
        {
            attackOrder = null;
            AttackTarget = null;
            GiveNewSubOrder();
        }
        
        void GiveNewSubOrder()
        {
            if (DestinationInRange)
            {
                AttackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
                if (AttackTarget != null)
                {
                    attackOrder = new AttackOrder(MapElement, AttackTarget);
                    GiveSubOrder(attackOrder);
                }
            }
            else
            {
                moveOrder = new MoveOrder(Unit, Destination, true);
                GiveSubOrder(moveOrder);
            }
        }

        void UpdateEscortData()
        {
            Targets.RemoveWhere(t => t.Dying);
            if (Targets.Empty())
            {
                if (moveOrder != null) moveOrder.Stop();
                if (attackOrder != null) attackOrder.Stop();
                Succeed();
            }
            else
            {
                Destination = Targets.Average(t => t.Coords).Round();
                if (moveOrder != null)
                    moveOrder.Destination = Destination;
            }
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("Targets: {0}, AttackTarget: {1}", Targets.ToDebugMessage(),
                AttackTarget != null ? AttackTarget.ToString() : "NONE");
        }
    }
}
