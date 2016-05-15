using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class HoverController
    {
        PlayerMouse mouse;
        
        bool preHoverBoxState;
        Vector2 hoverBoxStart;

        HashSet<MapElement> hoveredMapElements;
        public IEnumerable<MapElement> HoveredMapElements { get { return hoveredMapElements; } }

        public HoverBox HoverBox { get; private set; }
        public bool HoverBoxEnabled { get; private set; }
        public bool HoverBoxActive { get { return HoverBox != null; } }

        float hoverBoxMinDistance;
        public float HoverBoxMinDistance
        {
            get { return hoverBoxMinDistance; }
            set
            {
                if (value < 0)
                    throw new System.Exception("HoverBoxMinDistance must be greater or equal 0.");
                hoverBoxMinDistance = value;
            }
        }

        public HoverController(PlayerMouse mouse)
        {
            this.mouse = mouse;
            hoveredMapElements = new HashSet<MapElement>();
        }

        public void Update()
        {
            UpdateHoverBoxEnabled();

            var candidates = new HashSet<MapElement>();

            if (mouse.BehaviourDeterminant.AllowsHover)
            {
                UpdateRaycastHover(candidates);
                ManageHoverBox(candidates);
                FilterHoverCandidates(candidates);
            }
            UpdateHoveredMapElements(candidates);
        }

        void UpdateHoverBoxEnabled()
        {
            HoverBoxEnabled = mouse.BehaviourDeterminant.AllowsMultiTarget;
            if (!HoverBoxEnabled && HoverBoxActive)
                throw new System.Exception(
                    "HoverController is in invalid state - HoverBoxEnabled is false, but HoverBoxActive is true. \n" +
                    "Perhaps PlayerMouse.CarriedOrderAction changed while HoverBox was still active?");
        }
        
        void UpdateRaycastHover(HashSet<MapElement> candidates)
        {
            if (mouse.MapRaycast.MapElement != null)
                candidates.Add(mouse.MapRaycast.MapElement);
        }

        void ManageHoverBox(HashSet<MapElement> candidates)
        {
            if (HoverBoxEnabled)
            {
                if (!HoverBoxActive)
                {
                    if (mouse.Left.IsDown && !mouse.Right.IsPressed)
                    {
                        preHoverBoxState = true;
                        hoverBoxStart = mouse.Position;
                    }
                    if (preHoverBoxState)
                    {
                        var distance = Vector2.Distance(hoverBoxStart, mouse.Position);
                        if (distance > hoverBoxMinDistance)
                        {
                            preHoverBoxState = false;
                            HoverBox = new HoverBox(mouse, hoverBoxStart);
                        }
                    }
                }
                else
                {
                    HoverBox.Update();
                    candidates.UnionWith(HoverBox.MapElementsInside);
                    if (mouse.Left.IsUp)
                        HoverBox = null;
                }
            }
        }

        void FilterHoverCandidates(HashSet<MapElement> candidates)
        {
            mouse.BehaviourDeterminant.FilterHoverCandidates(mouse.Player, candidates);
        }

        void UpdateHoveredMapElements(HashSet<MapElement> candidates)
        {
            foreach (var me in hoveredMapElements)
                if (!candidates.Contains(me))
                    me.Hovered = false;
            hoveredMapElements.RemoveWhere(me => !me.Hovered);
            foreach (var me in candidates)
                if (!me.Hovered)
                {
                    me.Hovered = true;
                    hoveredMapElements.Add(me);
                }
        }
    }
}