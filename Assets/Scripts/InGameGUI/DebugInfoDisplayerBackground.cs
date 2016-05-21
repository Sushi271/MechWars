using UnityEngine;

namespace MechWars.InGameGUI
{
    public class DebugInfoDisplayerBackground : MonoBehaviour
    {
        public DebugInfoDisplayer debugInfoDisplayer;
        int padding = 3;

        void Update()
        {
            var thisTransform = GetComponent<RectTransform>();
            var transf = debugInfoDisplayer.GetComponent<RectTransform>();

            thisTransform.position = new Vector3(
                transf.position.x - padding,
                transf.position.y + padding,
                0);
            thisTransform.sizeDelta = new Vector2(
                transf.sizeDelta.x + padding * 2,
                transf.sizeDelta.y + padding * 2);
        }
    }
}
