using UnityEngine;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    public class ResourceCounter : MonoBehaviour
    {
        public Canvas canvas;

        void Update()
        {
            var text = gameObject.GetComponent<Text>();
            var player = canvas.GetComponent<CanvasScript>().thisPlayer;
            text.text = string.Format("{0} RP", player.Army.resources);
        }
    }
}
