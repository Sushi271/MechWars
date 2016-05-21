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

        protected override OrderExecutor CreateOrderExecutor()
        {
            var orderExecutor = new OrderExecutor(() => new IdleOrder(this));
            orderExecutor.GiveReplaces = true;
            return orderExecutor;
        }
        
        public override StringBuilder TEMP_PrintStatus()
        {
            return base.TEMP_PrintStatus().AppendLine()
                .Append(string.Format("Current order: {0}", OrderExecutor.CurrentOrder == null ? "NONE" : OrderExecutor.CurrentOrder.ToString()));
        }
    }
}