using MechWars.MapElements.Orders;
using MechWars.MapElements.Orders_OLD;
using System.Text;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        protected override bool CanAddToArmy { get { return true; } }
        public override bool Selectable { get { return true; } }
        public override bool CanBeAttacked { get { return true; } }
        public override bool CanBeEscorted { get { return true; } }

        public Move Move { get; private set; }

        protected override OrderExecutor CreateOrderExecutor(bool enabled = true)
        {
            var orderExecutor = base.CreateOrderExecutor();
            orderExecutor.GiveReplaces = true;
            return orderExecutor;
        }

        public bool SetMove(SingleMoveOrder singleMoveOrder)
        {
            if (Move != null || singleMoveOrder.State != OrderState.BrandNew)
                return false;
            Move = new Move(singleMoveOrder);
            return true;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Move != null)
            {
                Move.Update();
                if (Move.Done)
                    Move = null;
            }
        }

        public override StringBuilder DEBUG_PrintStatus(StringBuilder sb)
        {
            base.DEBUG_PrintStatus(sb)
                .AppendLine();
            DEBUG_PrintOrders(sb);
            return sb;
        }
    }
}