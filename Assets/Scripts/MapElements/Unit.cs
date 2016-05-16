using MechWars.MapElements.Orders;
using MechWars.MapElements.Orders.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        public bool canCollect;

        protected override bool CanAddToArmy { get { return true; } }
        public override bool Selectable { get { return true; } }
        public override bool CanBeAttacked { get { return true; } }
        public override bool CanBeEscorted { get { return true; } }
        
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isShadow) return;
        }
        
        public override StringBuilder TEMP_PrintStatus()
        {
            return base.TEMP_PrintStatus().AppendLine()
                .AppendLine(string.Format("Can collect resources: {0}", canCollect))
                .Append(string.Format("Current order: {0}", OrderExecutor.CurrentOrder == null ? "NONE" : OrderExecutor.CurrentOrder.ToString()));
        }
    }
}