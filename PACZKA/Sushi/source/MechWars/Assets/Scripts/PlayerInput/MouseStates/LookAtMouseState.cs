using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.PlayerInput.MouseStates
{
    public class LookAtMouseState : MouseState
    {
        public LookAtMouseState(MouseStateController stateController)
            : base(stateController)
        {
        }

        public override void FilterHoverCandidates( HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.LookAt(candidates);
        }
        
        public override void Handle()
        {
            if (StateController.LeftActionTriggered)
            {
                var hovered = InputController.HoverController.HoveredMapElements;
                var center = hovered.Average2(me => me.Coords);
                // TODO: move camera to center
            }
        }
    }
}
