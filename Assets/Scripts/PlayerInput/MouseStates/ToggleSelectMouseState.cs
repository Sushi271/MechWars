using MechWars.Human;
using MechWars.MapElements;
using System.Collections.Generic;

namespace MechWars.PlayerInput.MouseStates
{
    public class ToggleSelectMouseState : MouseState
    {
        static ToggleSelectMouseState instance;
        public static ToggleSelectMouseState Instance
        {
            get
            {
                if (instance == null)
                    instance = new ToggleSelectMouseState();
                return instance;
            }
        }

        ToggleSelectMouseState()
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Attack(player, candidates);
        }

        bool leftDown;
        public override void Handle(InputController inputController)
        {
            var hovered = inputController.HoverController.HoveredMapElements;

            if (inputController.Mouse.MouseStateLeft.IsDown) leftDown = true;
            if (inputController.Mouse.MouseStateRight.IsDown) leftDown = false;
            if (leftDown && inputController.Mouse.MouseStateLeft.IsUp)
            {
                inputController.SelectionMonitor.SelectOrToggle(hovered);
                leftDown = false;
            }
        }
    }
}
