using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;
using MechWars.Human;

namespace MechWars.PlayerInput.MouseStates
{
    public class AttackMouseState : MouseState
    {
        static AttackMouseState instance;
        public static AttackMouseState Instance
        {
            get
            {
                if (instance == null)
                    instance = new AttackMouseState();
                return instance;
            }
        }

        public override Color FramesColor { get { return Color.red; } }

        AttackMouseState()
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
