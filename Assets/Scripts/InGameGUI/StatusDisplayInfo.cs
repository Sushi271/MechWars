using MechWars.MapElements;
using UnityEngine;

namespace MechWars.InGameGUI
{
    public class StatusDisplayInfo
    {
        public MapElement MapElement { get; private set; }
        public bool Hovered { get; private set; }
        public bool Selected { get; private set; }

        public Vector2 Center { get; private set; }
        public Vector2 Size { get; private set; }
        public Vector2 Location { get; private set; }

        public float Left { get; private set; }
        public float Right { get; private set; }
        public float Top { get; private set; }
        public float Bottom { get; private set; }

        public float Width { get; private set; }
        public float Height { get; private set; }

        public float Distance { get; private set; }

        public StatusDisplayInfo(MapElement mapElement, bool hovered, bool selected)
        {
            MapElement = mapElement;
            Hovered = hovered;
            Selected = selected;

            var cam = Camera.main;
            Vector3 camPos = cam.transform.position;
            Quaternion camRot = cam.transform.rotation;
            Vector3 localZ = Vector3.forward;
            Vector3 vz = (camRot * localZ).normalized;
            Vector3 vx = Vector3.Cross(Vector3.up, vz).normalized;
            Vector3 vy = Vector3.Cross(vx, vz).normalized;

            if (mapElement.displaySize <= 0)
                throw new System.Exception(string.Format(
                    "Field \"Display Size\" on MapElement {0} must be greater than 0.", mapElement));

            try
            {
                float r = mapElement.displaySize / 2;
                Vector3 mapElementPos = mapElement.transform.position + Vector3.up * mapElement.displayYOffset;
                Vector3 top = mapElementPos + vy * r;
                Vector3 bottom = mapElementPos - vy * r;
                Vector3 left = mapElementPos - vx * r;
                Vector3 right = mapElementPos + vx * r;

                Top = Screen.height - cam.WorldToScreenPoint(top).y;
                Bottom = Screen.height - cam.WorldToScreenPoint(bottom).y;
                Left = cam.WorldToScreenPoint(left).x;
                Right = cam.WorldToScreenPoint(right).x;
                Width = Right - Left;
                Height = Top - Bottom;

                Vector3 center3 = cam.WorldToScreenPoint(mapElementPos);
                Center = new Vector2(center3.x, center3.y);
                Location = new Vector2(Left, Bottom);
                Size = new Vector2(Width, Height);

                Vector3 camToMapElement = mapElementPos - camPos;
                Distance = camToMapElement.magnitude;
            }
            catch (System.Exception e)
            {
                var exc = e;
                int x = 5;
                int y = x;
                throw e;
            }
        }
    }
}
