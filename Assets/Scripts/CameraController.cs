using UnityEngine;

namespace MechWars
{
    public class CameraController : MonoBehaviour
    {
        public GameObject handle;

        public int scrollMargin = 15;
        public float scrollSpeed = 10;
        public bool scrollingEnabled = true;

        float minZoom = 0.25f;
        float maxZoom = 4;
        public float zoom = 1;
        public float zoomFactor = 10;
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
                scrollingEnabled = !scrollingEnabled;

            if (scrollingEnabled)
            {
                var w = Screen.width;
                var h = Screen.height;
                var mp = Input.mousePosition;

                var rect = new Rect(scrollMargin, scrollMargin, w - 2 * scrollMargin, h - 2 * scrollMargin);
                if (!rect.Contains(mp))
                {
                    var vel = Vector3.zero;
                    if (mp.x < rect.xMin) vel += new Vector3(-1, 0, 1);
                    if (rect.xMax < mp.x) vel += new Vector3(1, 0, -1);
                    if (mp.y < rect.yMin) vel += new Vector3(-1, 0, -1);
                    if (rect.yMax < mp.y) vel += new Vector3(1, 0, 1);
                    vel *= scrollSpeed * zoom;

                    handle.transform.localPosition += Time.deltaTime * vel;
                    CorrectCameraPosition();
                }
            }

            var mwAxis = Input.GetAxis(Axes.MouseWheel);
            if (mwAxis != 0)
            {
                var dZoom = mwAxis * zoomFactor;
                var expDZoom = Mathf.Pow(0.9f, dZoom);
                var newZoom = zoom * expDZoom;
                if (newZoom > maxZoom) newZoom = maxZoom;
                if (newZoom < minZoom) newZoom = minZoom;
                zoom = newZoom;

                handle.transform.localScale = Vector3.one * zoom;
            }
        }

        void CorrectCameraPosition()
        {
            var center = handle.transform.position;
            var glb = Globals.Instance;

            var correction = Vector3.zero;
            if (center.x < 0) correction -= new Vector3(center.x, 0, 0);
            if (glb.MapWidth - 1 < center.x) correction -= new Vector3(center.x - (glb.MapWidth - 1), 0, 0);
            if (center.z < 0) correction -= new Vector3(0, 0, center.z);
            if (glb.MapHeight - 1 < center.z) correction -= new Vector3(0, 0, center.z - (glb.MapHeight - 1));

            handle.transform.position += correction;
        }
    }
}