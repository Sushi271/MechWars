using MechWars.MapElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.PlayerInput.MouseStates
{
    public class ToggleSelectMouseState : MouseState
    {
        public override Color FramesColor
        {
            get
            {
                var hovered = InputController.HoverController.HoveredMapElements;
                if (hovered.All(me => Globals.Spectator.InputController.SelectionMonitor.IsSelected(me)))
                    return new Color(0.75f, 0.75f, 0.75f);
                return Color.black;
            }
        }

        public override Color HoverBoxColor { get { return Color.black; } }

        public ToggleSelectMouseState(MouseStateController stateController)
            : base(stateController)
        {
        }

        public override void FilterHoverCandidates(HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.ToggleSelect(candidates);
        }
        
        public override void Handle()
        {
            if (StateController.LeftActionTriggered)
            {
                var hovered = InputController.HoverController.HoveredMapElements;
                InputController.SelectionMonitor.SelectOrToggle(hovered);
            }
        }
    }
}
