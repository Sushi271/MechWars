using UnityEngine;
using System.Collections;

namespace MechWars
{
    public class CameraController : MonoBehaviour
    {
        public int scrollMargin = 20;
        public float scrollSpeed = 20;
        public bool scrollingEnabled = true;

        public Vector3 Center
        {
            get
            {
                var v = transform.forward;
                var p = transform.position;
                var t = -p.y / v.y;
                return p + t * v;
            }
        }

        void Update()
        {
            if (scrollingEnabled)
            {
                var w = Screen.width;
                var h = Screen.height;
                var mp = Input.mousePosition;

                var rect = new Rect(scrollMargin, scrollMargin, w - 2 * scrollMargin, h - 2 * scrollMargin);
                if (!rect.Contains(mp))
                {
                    var vel = Vector3.zero;
                    if (mp.x < rect.xMin) vel -= new Vector3(1, 0, 0);
                    if (rect.xMax < mp.x) vel += new Vector3(1, 0, 0);
                    if (mp.y < rect.yMin) vel -= new Vector3(0, 0, 1);
                    if (rect.yMax < mp.y) vel += new Vector3(0, 0, 1);
                    vel *= scrollSpeed;

                    transform.localPosition += Time.deltaTime * vel;
                    CorrectCameraPosition();
                }
            }
        }

        void CorrectCameraPosition()
        {
            var center = Center;
            var glb = Globals.Instance;

            var correction = Vector3.zero;
            if (center.x < 0) correction -= new Vector3(center.x, 0, 0);
            if (glb.MapWidth - 1 < center.x) correction -= new Vector3(center.x - (glb.MapWidth - 1), 0, 0);
            if (center.z < 0) correction -= new Vector3(0, 0, center.z);
            if (glb.MapHeight - 1 < center.z) correction -= new Vector3(0, 0, center.z - (glb.MapHeight - 1));

            transform.position += correction;
        }
    }
}