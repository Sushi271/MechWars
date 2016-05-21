using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;
using MechWars.MapElements.Orders.Actions;

namespace MechWars.PlayerInput.MouseStates
{
    public class EscortMouseState : MouseState
    {
        public override Color FramesColor { get { return Color.blue; } }

        public EscortMouseState(MouseStateController stateController)
            : base(stateController)
        {
        }

        public override void FilterHoverCandidates(HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Escort(candidates);
        }

        public override void Handle()
        {
            if (StateController.LeftActionTriggered)
                GiveOrdersIfPossible(typeof(EscortOrderAction));
        }
    }
}
