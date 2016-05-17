using MechWars.Human;
using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;

namespace MechWars.PlayerInput.MouseStates
{
    public class LookAtMouseState : MouseState
    {
        static LookAtMouseState instance;
        public static LookAtMouseState Instance
        {
            get
            {
                if (instance == null)
                    instance = new LookAtMouseState();
                return instance;
            }
        }

        LookAtMouseState()
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.LookAt(player, candidates);
        }

        bool leftDown;
        public override void Handle(InputController inputController)
        {
            var hovered = inputController.HoverController.HoveredMapElements;

            if (inputController.Mouse.MouseStateLeft.IsDown) leftDown = true;
            if (inputController.Mouse.MouseStateRight.IsDown) leftDown = false;
            if (leftDown && inputController.Mouse.MouseStateLeft.IsUp)
            {
                var center = hovered.Average(me => me.Coords);
                // TODO: move camera to center
                leftDown = false;
            }
        }
    }
}
