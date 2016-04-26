using UnityEngine;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    public class StatusDisplayerBackground : MonoBehaviour
    {
        public StatusDisplayer statusDisplayer;
        int padding = 3;

        void Update()
        {
            var thisTransform = GetComponent<RectTransform>();
            var statusDisplayerTransform = statusDisplayer.GetComponent<RectTransform>();

            thisTransform.position = new Vector3(
                statusDisplayerTransform.position.x - padding,
                statusDisplayerTransform.position.y + padding,
                0);
            thisTransform.sizeDelta = new Vector2(
                statusDisplayerTransform.sizeDelta.x + padding * 2,
                statusDisplayerTransform.sizeDelta.y + padding * 2);
        }
    }
}
