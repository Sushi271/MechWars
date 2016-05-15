using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;
using MechWars.Human;

namespace MechWars.PlayerInput.MouseStates
{
    public class EscortMouseState : MouseState
    {
        static EscortMouseState instance;
        public static EscortMouseState Instance
        {
            get
            {
                if (instance == null)
                    instance = new EscortMouseState();
                return instance;
            }
        }

        public override Color FramesColor { get { return Color.blue; } }

        EscortMouseState()
        {
        }

        public override void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates)
        {
            HoverCandidatesFilter.Escort(player, candidates);
        }

        public override void Handle(InputController inputController)
        {

        }
    }
}
