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

        public override void Handle(PlayerMouse mouse)
        {

        }
    }
}
