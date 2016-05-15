using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;
using MechWars.Human;

namespace MechWars.PlayerInput.MouseStates
{
    public abstract class MouseState : IMouseBehaviourDeterminant
    {
        public bool AllowsHover { get { return true; } }
        public bool AllowsMultiTarget { get { return true; } }
        public virtual Color FramesColor { get { return Color.black; } }

        public abstract void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates);
        public abstract void Handle(InputController inputController);
    }
}
