using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.PlayerInput
{
    public class SelectionMonitor
    {
        HashSet<MapElement> selectedMapElements;
        public IEnumerable<MapElement> SelectedMapElements { get { return selectedMapElements; } }
        public int SelectedCount { get { return selectedMapElements.Count; } }

        public SelectionMonitor()
        {
            selectedMapElements = new HashSet<MapElement>();
        }

        public bool IsSelected(MapElement mapElement)
        {
            return selectedMapElements.Contains(mapElement);
        }

        public void Select(IEnumerable<MapElement> mapElements)
        {
            AssertMapElementsSelectable(mapElements);

            var toAdd = new HashSet<MapElement>(mapElements);
            toAdd.ExceptWith(selectedMapElements);
            foreach (var me in toAdd)
                me.LifeEnding += MapElement_LifeEnding;
            selectedMapElements.UnionWith(toAdd);
        }

        public void Deselect(IEnumerable<MapElement> mapElements)
        {
            AssertMapElementsSelectable(mapElements);

            var toRemove = new HashSet<MapElement>(mapElements);
            toRemove.Intersect(selectedMapElements);
            foreach (var me in toRemove)
                me.LifeEnding -= MapElement_LifeEnding;
            selectedMapElements.ExceptWith(mapElements);
        }

        public void SelectNew(IEnumerable<MapElement> mapElements)
        {
            AssertMapElementsSelectable(mapElements);
            
            ClearSelection();
            Select(mapElements);
        }

        public void SelectOrToggle(IEnumerable<MapElement> mapElements)
        {
            AssertMapElementsSelectable(mapElements);

            if (selectedMapElements.IsSupersetOf(mapElements))
                Deselect(mapElements);
            else Select(mapElements);
        }

        public void ClearSelection()
        {
            foreach (var me in selectedMapElements)
                me.LifeEnding -= MapElement_LifeEnding;
            selectedMapElements.Clear();
        }

        private void MapElement_LifeEnding(MapElement sender)
        {
            Deselect(sender.AsEnumerable());
        }

        void AssertMapElementsSelectable(IEnumerable<MapElement> mapElements)
        {
            var nonSelectable = mapElements.Where(me => !me.Selectable);
            if (!nonSelectable.Empty())
                throw new System.Exception(string.Format("Following MapElements cannot be selected: {0}.",
                    nonSelectable.ToDebugMessage()));
        }
    }
}