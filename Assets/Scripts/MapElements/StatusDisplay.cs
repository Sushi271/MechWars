using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements
{
    public class StatusDisplay
    {
        MapElement mapElement;
        StatusDisplayActionDelegate statusDisplayAction;

        public StatusDisplay(MapElement parentMapElement, StatusDisplayActionDelegate statusDisplayAction)
        {
            mapElement = parentMapElement;
            this.statusDisplayAction = statusDisplayAction;
        }

        public void Draw()
        {
            if (!(mapElement.Hovered || mapElement.Selected || mapElement.InSelectionBox)) return;

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

            float r = mapElement.displaySize / 2;
            Vector3 mapElementPos = mapElement.transform.position + Vector3.up * mapElement.displayYOffset;
            Vector3 top = mapElementPos + vy * r;
            Vector3 bottom = mapElementPos - vy * r;
            Vector3 left = mapElementPos - vx * r;
            Vector3 right = mapElementPos + vx * r;

            float scrTop = Screen.height - cam.WorldToScreenPoint(top).y;
            float scrBottom = Screen.height - cam.WorldToScreenPoint(bottom).y;
            float scrLeft = cam.WorldToScreenPoint(left).x;
            float scrRight = cam.WorldToScreenPoint(right).x;
            Vector3 scrCenter = cam.WorldToScreenPoint(mapElementPos);

            int centerX = (int)scrCenter.x;
            int centerY = (int)scrCenter.y;
            int displayWidth = (int)(scrRight - scrLeft);
            int displayHeight = (int)(scrTop - scrBottom);

            Vector3 camToMapElement = mapElementPos - camPos;
            float distance = camToMapElement.magnitude;

            statusDisplayAction(centerX, centerY, displayWidth, displayHeight, distance);
        }
    }

    public delegate void StatusDisplayActionDelegate(int centerX, int centerY, int displayWidth, int displayHeight, float distance);
}
