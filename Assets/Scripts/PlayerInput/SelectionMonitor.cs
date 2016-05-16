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
            {
                me.Selected = false;
                selectedMapElements.Remove(me);
            }
        }

        public void SelectNew(IEnumerable<MapElement> mapElements)
        {
            AssertMapElementsSelectable(mapElements);

            foreach (var me in mapElements)
                if (me.Selected)
                    selectedMapElements.Remove(me); // DON'T set me.Selected = false!
            ClearSelection();                       // only those who are left have now set Selected = false
            foreach (var me in mapElements)
            {
                if (!me.Selected)
                    me.Selected = true;      // now setting Selected = true only for those, who were previously removed
                selectedMapElements.Add(me); // and all are added to selection
            }
        }

        public void ToggleSelect(IEnumerable<MapElement> mapElements)
        {
            AssertMapElementsSelectable(mapElements);

            foreach (var me in mapElements)
            {
                me.Selected = !me.Selected;
                if (me.Selected)
                    selectedMapElements.Add(me);
                else selectedMapElements.Remove(me);
            }
        }

        public void ClearSelection()
        {
            foreach (var me in selectedMapElements)
                me.Selected = false;
            selectedMapElements.RemoveWhere(me => !me.Selected);
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