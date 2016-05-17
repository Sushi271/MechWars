using MechWars.Human;
using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.PlayerInput.MouseStates
{
    public class LookAtMouseState : MouseState
    {
        public LookAtMouseState(InputController inputController)
            : base(inputController)
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.LookAt(player, candidates);
        }

        bool leftDown;
        public override void Handle()
        {
            var hovered = InputController.HoverController.HoveredMapElements;

            if (InputController.Mouse.MouseStateLeft.IsDown) leftDown = true;
            if (InputController.Mouse.MouseStateRight.IsDown) leftDown = false;
            if (leftDown && InputController.Mouse.MouseStateLeft.IsUp)
            {
                var center = hovered.Average(me => me.Coords);
                // TODO: move camera to center
                leftDown = false;
            }
        }
    }
}
