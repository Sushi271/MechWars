using MechWars.Human;
using MechWars.MapElements;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public interface IMouseBehaviourDeterminant
    {
        Color FramesColor { get; }
        bool AllowsMultiTarget { get; }
        bool AllowsHover { get; }

        void FilterHoverCandidates(HumanPlayer player, HashSet<MapElement> candidates);
    }
}
