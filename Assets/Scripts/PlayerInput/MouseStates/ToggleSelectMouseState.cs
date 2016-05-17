using MechWars.Human;
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
                if (hovered.All(me => me.Selected))
                    return new Color(0.75f, 0.75f, 0.75f);
                return Color.black;
            }
        }

        public override Color HoverBoxColor { get { return Color.black; } }

        public ToggleSelectMouseState(InputController inputController)
            : base(inputController)
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.ToggleSelect(player, candidates);
        }

        bool leftDown;
        public override void Handle()
        {
            var hovered = InputController.HoverController.HoveredMapElements;

            if (InputController.Mouse.MouseStateLeft.IsDown) leftDown = true;
            if (InputController.Mouse.MouseStateRight.IsDown) leftDown = false;
            if (leftDown && InputController.Mouse.MouseStateLeft.IsUp)
            {
                InputController.SelectionMonitor.SelectOrToggle(hovered);
                leftDown = false;
            }
        }
    }
}
