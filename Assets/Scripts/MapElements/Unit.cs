using MechWars.MapElements.Orders;
using System.Text;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        protected override bool CanAddToArmy { get { return true; } }
        public override bool Selectable { get { return true; } }
        public override bool CanBeAttacked { get { return true; } }
        public override bool CanBeEscorted { get { return true; } }

        protected override OrderExecutor CreateOrderExecutor(bool enabled = true)
        {
            var orderExecutor = base.CreateOrderExecutor();
            orderExecutor.GiveReplaces = true;
            return orderExecutor;
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