using UnityEngine;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    public class ResourceCounter : MonoBehaviour
    {
        public Canvas canvas;
        public Army army;

        void Update()
        {
            var text = gameObject.GetComponent<Text>();
            text.text = string.Format("{0} RUs", army != null ? army.resources.ToString() : "- - -");
        }
    }
}
