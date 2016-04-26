using MechWars.Human;
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
                var mapElements = hp.SelectionController.SelectedMapElements;
                if (mapElements.Count > 1)
                    text.text = string.Format("Selected {0} MapElements.", mapElements.Count);
                else if (mapElements.Count == 0)
                    text.text = "No selection.";
                else text.text = mapElements.First().TEMP_PrintStatus();
            }
        }
    }
}
