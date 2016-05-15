using MechWars.Human;
using MechWars.MapElements;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.PlayerInput.MouseStates
{
    public class DefaultMouseState : MouseState
    {
        static DefaultMouseState instance;
        public static DefaultMouseState Instance
        {
            get
            {
                if (instance == null)
                    instance = new DefaultMouseState();
                return instance;
            }
        }

        DefaultMouseState()
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Select(player, candidates);
        }

        public override void Handle(PlayerMouse mouse)
        {

        }
    }
}
