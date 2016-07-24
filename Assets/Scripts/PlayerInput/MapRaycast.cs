using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MechWars.PlayerInput
{
    public class MapRaycast
    {
        PlayerMouse mouse;

        public bool GUIHit { get; private set; }
        public MapElement MapElement { get; private set; }
        public Vector2? PreciseCoords { get; private set; }
        public IVector2? Coords { get; private set; }

        public MapRaycast(PlayerMouse mouse)
        {
            this.mouse = mouse;
        }

        public void Update()
        {
            var ray = Camera.main.ScreenPointToRay(mouse.Position);

            RaycastHit hitInfo;
            
            GUIHit = false;
            MapElement = null;
            PreciseCoords = null;
            Coords = null;

            var pointerData = new PointerEventData(EventSystem.current) { position = mouse.Position };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask(Layer.UI));
            
            if (!results.Empty())
                GUIHit = true;
            else if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask(Layer.MapElements)))
            {
                var go = hitInfo.collider.gameObject;
                MapElement = go.GetComponentInParent<MapElement>();
                if (MapElement == null)
                    throw new System.Exception("Non-MapElement GameObject is in MapElements layer.");
                PreciseCoords = MapElement.Coords;
                Coords = PreciseCoords.Value.Round();
            }
            else if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask(Layer.Terrain)))
            {
                PreciseCoords = new Vector2(hitInfo.point.x, hitInfo.point.z);
                Coords = PreciseCoords.Value.Round();
                MapElement = Globals.Map[Coords.Value];
            }
        }
    }
}