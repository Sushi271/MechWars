using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.MapElements.Orders
{
    public class AttackMoveOrder : ComplexOrder
    {
        public override string Name { get { return "AttackMove"; } }

        public Unit Unit { get; private set; }

        public MapElement AttackTarget { get; private set; }
        IVector2 AttackTargetClosestCoords { get { return AttackTarget.GetClosestFieldTo(Unit.Coords); } }

        IVector2 Destination { get; set; }

        MoveOrder moveOrder;
        FollowAttackOrder followAttackOrder;

        public AttackMoveOrder(Unit unit, IVector2 destination)
            : base(unit)
        {
            Unit = unit;
            Destination = destination;
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertMapElementHasAnyAttacks(MapElement));
            if (Failed) return;

            GiveNewSubOrder();
        }

        protected override void OnSubOrderUpdating()
        {
            if (SubOrder.State == OrderState.Stopping) return;

            if (SubOrder == moveOrder)
            {
                AttackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
                if (AttackTarget != null)
                    moveOrder.Stop();
            }
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == moveOrder) moveOrder = null;
            else if (SubOrder == followAttackOrder)
            {
                followAttackOrder = null;
                AttackTarget = null;
            }

            if (State != OrderState.Stopping && !Conclusive)
                GiveNewSubOrder();
        }

        protected override void OnSubOrderFinished()
        {
            if (SubOrder == moveOrder)
            {
                moveOrder = null;
                Succeed();
            }
            else if (SubOrder == followAttackOrder)
            {
                followAttackOrder = null;
                AttackTarget = null;
                GiveNewSubOrder();
            }
        }
        
        void GiveNewSubOrder()
        {
            AttackTarget = MapElement.PickClosestEnemyInRange(StatNames.AttackRange);
            if (AttackTarget != null)
            {
                followAttackOrder = new FollowAttackOrder(Unit, AttackTarget.AsEnumerable());
                GiveSubOrder(followAttackOrder);
            }
            else
            {
                moveOrder = new MoveOrder(Unit, Destination);
                GiveSubOrder(moveOrder);
            }
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("Destination: {0}, AttackTarget: {1}", Destination,
                AttackTarget != null ? AttackTarget.ToString() : "NONE");
        }
    }
}
