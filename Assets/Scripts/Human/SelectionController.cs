using System.Collections.Generic;
using System.Linq;
using MechWars.MapElements;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.Human
{
    public class SelectionController
    {
        HumanPlayer player;

        MapElement singleHoveredMapElement;
        MapElement singleSelectionCandidate;
        SelectionBox selectionBox;

        public HashSet<MapElement> HoveredMapElements { get; private set; }
        public HashSet<MapElement> SelectedMapElements { get; private set; }

        bool MultiSelection { get { return selectionBox != null; } }

        public SelectionController(HumanPlayer player)
        {
            this.player = player;
            HoveredMapElements = new HashSet<MapElement>();
            SelectedMapElements = new HashSet<MapElement>();
        }

        public void Update()
        {
            bool toggleSelection =
                Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift);

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            singleHoveredMapElement = null;
            if (Physics.Raycast(ray, out hit))
            {
                var go = hit.collider.gameObject;
                if (go != null)
                {
                    var mapElement = go.GetComponentInParent<MapElement>();
                    if (mapElement != null && mapElement.selectable)
                        singleHoveredMapElement = mapElement;
                }
            }

            if (!MultiSelection)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    singleSelectionCandidate = singleHoveredMapElement;
                    if (singleSelectionCandidate == null)
                    {
                        selectionBox = new SelectionBox
                        {
                            Position = Input.mousePosition,
                            Size = Vector2.zero
                        };
                    }
                }
            }

            IEnumerable<MapElement> candidates = new HashSet<MapElement>();
            if (singleHoveredMapElement != null)
                candidates = candidates.Concat(singleHoveredMapElement.AsEnumerable());

            if (MultiSelection)
            {
                selectionBox.Update();
                selectionBox.Draw();
                candidates = candidates.Concat(selectionBox.MapElementsInside);
            }

            var newHoveredMapElements = FilterCandidates(candidates);
            UpdateHoveredMapElements(newHoveredMapElements);

            if (Input.GetMouseButtonUp(0))
            {
                if (!toggleSelection) DeselectAllMapElements();

                if (!MultiSelection)
                {
                    if (HoveredMapElements.Count() > 0 &&
                        singleSelectionCandidate == HoveredMapElements.Single())
                    {
                        if (toggleSelection) ToggleSelectMapElement(singleSelectionCandidate);
                        else SelectMapElement(singleSelectionCandidate);

                        singleSelectionCandidate = null;
                    }
                }
                else
                {
                    if (toggleSelection && HoveredMapElements.All(me => SelectedMapElements.Contains(me)))
                        DeselectMapElements(HoveredMapElements);
                    else SelectMapElements(HoveredMapElements);

                    selectionBox = null;
                }
            }
        }

        IEnumerable<MapElement> FilterCandidates(IEnumerable<MapElement> mapElements)
        {
            var hoveredMapElements = new HashSet<MapElement>();

            var selectableMapElements = mapElements.Where(me => me.selectable);
            var units = selectableMapElements.Where(me => me is Unit).Cast<Unit>();
            var thisPlayersUnits = units.Where(u => u.army != null && u.army.Player == player);

            if (thisPlayersUnits.Count() > 0)
            {
                foreach (var u in thisPlayersUnits)
                    hoveredMapElements.Add(u);
            }
            else if (units.Count() > 0)
                hoveredMapElements.Add(units.First());
            else if (selectableMapElements.Count() > 0)
                hoveredMapElements.Add(selectableMapElements.First());

            return hoveredMapElements;
        }

        void UpdateHoveredMapElements(IEnumerable<MapElement> newHoveredMapElements)
        {
            foreach (var me in HoveredMapElements)
            {
                if (!newHoveredMapElements.Contains(me))
                    me.Hovered = false;
            }
            HoveredMapElements.RemoveWhere(me => !me.Hovered);
            foreach (var me in newHoveredMapElements)
            {
                if (!HoveredMapElements.Contains(me))
                {
                    me.Hovered = true;
                    HoveredMapElements.Add(me);
                }
            }
        }

        void DeselectAllMapElements()
        {
            foreach (var a in SelectedMapElements)
            {
                a.Selected = false;
            }
            SelectedMapElements.Clear();
        }

        void SelectMapElement(MapElement mapElement)
        {
            SelectedMapElements.Add(mapElement);
            mapElement.Selected = true;
        }

        void SelectMapElements(IEnumerable<MapElement> mapElements)
        {
            foreach (var a in mapElements)
            {
                SelectMapElement(a);
            }
        }

        void DeselectMapElement(MapElement mapElement)
        {
            mapElement.Selected = false;
            SelectedMapElements.Remove(mapElement);
        }

        void DeselectMapElements(IEnumerable<MapElement> mapElements)
        {
            foreach (var a in mapElements)
            {
                DeselectMapElement(a);
            }
        }

        void ToggleSelectMapElement(MapElement mapElement)
        {
            if (mapElement.Selected) DeselectMapElement(mapElement);
            else SelectMapElement(mapElement);
        }
    }
}