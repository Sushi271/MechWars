using MechWars.MapElements;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public interface IMouseBehaviourDeterminant
    {
        Color FramesColor { get; }
        Color HoverBoxColor { get; }
        bool AllowsMultiTarget { get; }
        bool AllowsHover { get; }

        void FilterHoverCandidates(HashSet<MapElement> candidates);
    }
}
