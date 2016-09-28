using MechWars.Utils;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    // InfoDisplayer z debugowym tekstem
    public class DebugInfoDisplayer : InfoDisplayer
    {
        void Update()
        {
            var text = GetComponent<Text>();
            var spectator = Globals.Spectator;
            if (spectator != null)
            {
                var mapElements = spectator.InputController.SelectionMonitor.SelectedMapElements;
                if (mapElements.HasAtLeast(2))
                    text.text = string.Format("Selected {0} MapElements.", mapElements.Count());
                else if (mapElements.Empty())
                    text.text = "No selection.";
                else text.text = mapElements.First().DEBUG_PrintStatus(new StringBuilder()).ToString();
            }
        }
    }
}
