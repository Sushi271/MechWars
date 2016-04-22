using MechWars.MapElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars
{
    public class StatusDisplayer : MonoBehaviour
    {
        HashSet<StatusDisplay> displays;

        public StatusDisplayer()
        {
            displays = new HashSet<StatusDisplay>();
        }

        public void DisplayStatusFor(MapElement mapElement)
        {
            displays.Add(new StatusDisplay(mapElement));
        }

        void OnGUI()
        {
            displays.RemoveWhere(sd => sd.Canceled);
            if (displays.Count == 0) return;
            foreach (var d in displays)
            {
                d.CalculateDimensions();
            }

            var displayList = displays.ToList();
            displayList.Sort((d1, d2) => d2.Distance.CompareTo(d1.Distance));
            var displayDepth = 1.0f / displayList.Count;
            var depthSoFar = 0f;

            foreach (var d in displayList)
            {
                d.Near = depthSoFar;
                d.Depth = displayDepth;
                depthSoFar += displayDepth;
                d.Draw();
            }

            displays.Clear();
        }
    }
}
