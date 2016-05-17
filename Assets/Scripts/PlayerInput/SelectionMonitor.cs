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

        public SelectionMonitor()
        {
            selectedMapElements = new HashSet<MapElement>();
        }

        public void Select(IEnumerable<MapElement> mapElements)
        {
            AssertMapElementsSelectable(mapElements);

            foreach (var me in mapElements)
            {
                me.Selected = true;
                selectedMapElements.Add(me);
            }
        }

        public void Deselect(IEnumerable<MapElement> mapElements)
        {
            AssertMapElementsSelectable(mapElements);

            foreach (var me in mapElements)
                me.Selected = false;
            selectedMapElements.RemoveWhere(me => !me.Selected);
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

            if (mapElements.All(me => me.Selected))
                Deselect(mapElements);
            else Select(mapElements);
        }

        public void ClearSelection()
        {
            Deselect(selectedMapElements);
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