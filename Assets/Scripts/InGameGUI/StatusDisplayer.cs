using MechWars.Human;
using MechWars.Utils;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    public class StatusDisplayer : MonoBehaviour
    {
        public Canvas canvas;

        void Update()
        {
            var text = GetComponent<Text>();
            var p = canvas.GetComponent<CanvasScript>().thisPlayer;
            if (p != null && p is HumanPlayer)
            {
                var hp = (HumanPlayer)p;
                var mapElements = hp.InputController.SelectionMonitor.SelectedMapElements;
                if (mapElements.HasAtLeast(2))
                    text.text = string.Format("Selected {0} MapElements.", mapElements.Count());
                else if (mapElements.Empty())
                    text.text = "No selection.";
                else text.text = mapElements.First().TEMP_PrintStatus().ToString();
            }
        }
    }
}
