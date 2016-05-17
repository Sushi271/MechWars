using MechWars.Human;
using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.PlayerInput.MouseStates
{
    public abstract class MouseState : IMouseBehaviourDeterminant
    {
        public bool AllowsHover { get { return true; } }
        public bool AllowsMultiTarget { get { return true; } }
        public virtual Color FramesColor { get { return Color.black; } }

        public abstract void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates);
        public abstract void Handle(InputController inputController);
        
        protected void GiveOrdersIfPossible(InputController inputController, IEnumerable<MapElement> targets, params System.Type[] types)
        {
            var args = new OrderActionArgs(inputController.MapRaycast.Coords.Value, targets);
            var selected = inputController.SelectionMonitor.SelectedMapElements;
            foreach (var me in selected)
            {
                if (me.army != inputController.Player.Army) continue;

                var result = me.orderActions.FirstOrAnother(types.Select(
                    t => new System.Func<OrderAction, bool>(oa => oa.GetType() == t)).ToArray());
                if (!result.Found) continue;

                var orderAction = result.Result;
                if (me.OrderExecutor.Enabled)
                    me.OrderExecutor.Give(orderAction.CreateOrder(me, args));
            }
        }
    }
}
