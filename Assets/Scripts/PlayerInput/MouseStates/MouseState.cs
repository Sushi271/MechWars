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
        protected InputController InputController { get { return StateController.InputController; } }

        protected MouseStateController StateController { get; private set; }

        public bool AllowsHover { get { return true; } }
        public bool AllowsMultiTarget { get { return true; } }
        public virtual Color FramesColor { get { return Color.black; } }
        public virtual Color HoverBoxColor { get { return FramesColor; } }

        public MouseState(MouseStateController stateController)
        {
            StateController = stateController;
        }

        public abstract void FilterHoverCandidates(HashSet<MapElement> candidates);
        public abstract void Handle();
        
        protected void GiveOrdersIfPossible(params System.Type[] types)
        {
            var selected = InputController.SelectionMonitor.SelectedMapElements;
            foreach (var me in selected)
            {
                if (me.army != Globals.HumanArmy) continue;

                var result = me.orderActions.FirstOrAnother(types.Select(
                    t => new System.Func<OrderAction, bool>(oa => oa.GetType() == t)).ToArray());
                if (!result.Found) continue;

                var orderAction = result.Result;
                orderAction.GiveOrder(me, InputController.OrderActionArgs);
            }
        }
    }
}
