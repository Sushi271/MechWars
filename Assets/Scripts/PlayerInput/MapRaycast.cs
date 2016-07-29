using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MechWars.PlayerInput
{
    public class MapRaycast
    {
        PlayerMouse mouse;

        public bool GUIHit { get; private set; }
        public MapElement MapElement { get; private set; }
        public MapElement[] Ghosts { get; private set; }
        public Vector2? PreciseCoords { get; private set; }
        public IVector2? Coords { get; private set; }

        public MapRaycast(PlayerMouse mouse)
        {
            this.mouse = mouse;
            Ghosts = new MapElement[0];
        }

        public void Update()
        {
            var ray = Camera.main.ScreenPointToRay(mouse.Position);
            
            GUIHit = false;
            MapElement = null;
            Ghosts = new MapElement[0];
            PreciseCoords = null;
            Coords = null;

            var pointerData = new PointerEventData(EventSystem.current) { position = mouse.Position };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            if (!results.Empty())
            {
                GUIHit = true;
                return;
            }

            RaycastHit[] hitInfos = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask(Layer.MapElements));
            if (hitInfos.Length > 0)
            {
                var mapElements = hitInfos.SelectDistinct(
                    hi => hi.collider.gameObject.GetComponentInParent<MapElement>()).ToArray();
                if (mapElements.Any(me => me == null))
                    throw new System.Exception("Non-MapElement GameObject is in MapElements layer.");
                mapElements = mapElements.Where(me => me.VisibleToSpectator).ToArray();

                var closestHI = hitInfos.SelectMin(hi => hi.distance);
                var closestME = closestHI.collider.gameObject.GetComponentInParent<MapElement>();
                mapElements = mapElements.Where(me => me.id == closestME.id).ToArray();

                PreciseCoords = new Vector2(closestHI.point.x, closestHI.point.z);
                Coords = PreciseCoords.Value.Round();
                MapElement = mapElements.FirstOrDefault(me => !me.IsGhost);
                Ghosts = mapElements.Where(me => me.IsGhost).ToArray();

                return;
            }

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask(Layer.Terrain)))
            {
                PreciseCoords = new Vector2(hitInfo.point.x, hitInfo.point.z);
                Coords = PreciseCoords.Value.Round();
                MapElement = Globals.Map[Coords.Value];
                Ghosts = Globals.Map.GetGhosts(Coords.Value.X, Coords.Value.Y).ToArray();

                if (MapElement != null && !MapElement.VisibleToSpectator)
                    MapElement = null;
            }
        }
    }
}