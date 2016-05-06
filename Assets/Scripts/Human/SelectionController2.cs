using System.Collections.Generic;
using System.Linq;
using MechWars.MapElements;
using MechWars.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MechWars.Human
{
    public class SelectionController2
    {
        HumanPlayer player;

        MapElement singleHoveredMapElement;
        MapElement singleSelectionCandidate;
        SelectionBox selectionBox;

        public HashSet<MapElement> HoveredMapElements { get; private set; }
        public HashSet<MapElement> SelectedMapElements { get; private set; }

        bool multiSelection;
        bool toggleSelection;

        bool mouseDown;

        public SelectionController2(HumanPlayer player)
        {
            this.player = player;
            HoveredMapElements = new HashSet<MapElement>();
            SelectedMapElements = new HashSet<MapElement>();
        }

        public void Update()
        {
            HoveredMapElements.RemoveWhere(me => !me.Alive);
            SelectedMapElements.RemoveWhere(me => !me.Alive);

            if (player.MouseMode != MouseMode.Default ||
                EventSystem.current.IsPointerOverGameObject())
                return;

            toggleSelection =
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
                    if (mapElement == null && go.tag == Tag.Terrain)
                    {
                        var dest = new Vector2(hit.point.x, hit.point.z).Round();
                        mapElement = Globals.FieldReservationMap[dest];
                    }
                    if (mapElement != null && mapElement.selectable)
                        singleHoveredMapElement = mapElement;
                }
            }

            if (!multiSelection)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mouseDown = true;

                    singleSelectionCandidate = singleHoveredMapElement;
                    selectionBox = new SelectionBox
                    {
                        Position = Input.mousePosition,
                        Size = Vector2.zero
                    };
                    if (singleSelectionCandidate == null)
                        multiSelection = true;
                }
            }

            IEnumerable<MapElement> candidates = new HashSet<MapElement>();
            if (singleHoveredMapElement != null)
                candidates = candidates.Concat(singleHoveredMapElement.AsEnumerable());

            if (selectionBox != null)
            {
                selectionBox.Update();
                if (!multiSelection && selectionBox.Size.magnitude > 10)
                    multiSelection = true;
                if (multiSelection)
                {
                    selectionBox.Draw();
                    candidates = candidates.Concat(selectionBox.MapElementsInside);
                }
            }

            var newHoveredMapElements = FilterCandidates(candidates);
            UpdateHoveredMapElements(newHoveredMapElements);

            if (mouseDown && Input.GetMouseButtonUp(0))
            {
                mouseDown = false;

                if (!toggleSelection) DeselectAllMapElements();

                if (!multiSelection)
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

                    multiSelection = false;
                }
                selectionBox = null;
            }
        }

        IEnumerable<MapElement> FilterCandidates(IEnumerable<MapElement> mapElements)
        {
            var hoveredMapElements = new HashSet<MapElement>();

            var selectableMapElements = mapElements.Where(me => me.selectable);
            if (selectableMapElements.Count() == 0)
                return hoveredMapElements;

            var armies = selectableMapElements.Select(me => me.army).Distinct();
            var army = armies.Contains(player.Army) ? player.Army : armies.FirstOrDefault(a => a != null);
            var armyMapElements = selectableMapElements.Where(me => me.army == army);

            var units = armyMapElements.Where(me => me is Unit).Cast<Unit>();
            var buildings = armyMapElements.Where(me => me is Building).Cast<Building>();
            var attackingBuildings = buildings.Where(b => b.canAttack);
            
            if (units.Count() > 0 || attackingBuildings.Count() > 0)
            {
                foreach (var u in units)
                    hoveredMapElements.Add(u);
                foreach (var b in attackingBuildings)
                    hoveredMapElements.Add(b);
            }
            else if (buildings.Count() > 0)
            {
                string name = buildings.First().mapElementName;
                var namedBuildings = buildings.Where(b => b.mapElementName == name);
                foreach (var b in namedBuildings)
                    hoveredMapElements.Add(b);
            }
            else hoveredMapElements.Add(armyMapElements.First());            

            return hoveredMapElements;
        }

        void UpdateHoveredMapElements(IEnumerable<MapElement> newHoveredMapElements)
        {
            foreach (var me in HoveredMapElements)
                if (!newHoveredMapElements.Contains(me))
                    me.Hovered = false;
            HoveredMapElements.RemoveWhere(me => !me.Hovered);

            foreach (var me in newHoveredMapElements)
                if (!HoveredMapElements.Contains(me))
                {
                    me.Hovered = true;
                    HoveredMapElements.Add(me);
                }
        }

        public void DeselectAllMapElements()
        {
            foreach (var a in SelectedMapElements)
                a.Selected = false;
            SelectedMapElements.Clear();
        }

        public void SelectMapElement(MapElement mapElement)
        {
            SelectedMapElements.Add(mapElement);
            mapElement.Selected = true;
        }

        public void SelectMapElements(IEnumerable<MapElement> mapElements)
        {
            foreach (var a in mapElements)
                SelectMapElement(a);
        }

        public void DeselectMapElement(MapElement mapElement)
        {
            mapElement.Selected = false;
            SelectedMapElements.Remove(mapElement);
        }

        public void DeselectMapElements(IEnumerable<MapElement> mapElements)
        {
            foreach (var a in mapElements)
                DeselectMapElement(a);
        }

        public void ToggleSelectMapElement(MapElement mapElement)
        {
            if (mapElement.Selected) DeselectMapElement(mapElement);
            else SelectMapElement(mapElement);
        }
    }
}