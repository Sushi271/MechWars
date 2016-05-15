using MechWars.Human;
using MechWars.MapElements;
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

        public override void Handle(InputController inputController)
        {

        }
    }
}
